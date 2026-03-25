#!/usr/bin/env python3
"""
Convert Beamer HANDOUT PDFs to Markdown.
- Extracts text with proper heading/bullet/paragraph structure
- Detects diagram-heavy pages and renders them as PNG images
- Links extracted images in the Markdown
"""

import os
import re
import fitz  # PyMuPDF

DIAGRAM_WORD_THRESHOLD = 40
DIAGRAM_DRAW_MIN = 10
IMG_MIN_BBOX_SIDE = 15

# PyMuPDF span flag bits
_ITALIC_BIT = 2
_BOLD_BIT = 16
_MONO_BIT = 8


def _is_bold(flags: int) -> bool:
    return bool(flags & _BOLD_BIT)


def _is_italic(flags: int) -> bool:
    return bool(flags & _ITALIC_BIT)


def _is_mono(flags: int) -> bool:
    return bool(flags & _MONO_BIT)


def _decorate(text: str, flags: int) -> str:
    if not text.strip():
        return text
    bold = _is_bold(flags)
    italic = _is_italic(flags)
    mono = _is_mono(flags)
    if mono:
        return f"`{text}`"
    if bold and italic:
        return f"***{text}***"
    if bold:
        return f"**{text}**"
    if italic:
        return f"*{text}*"
    return text


def _line_to_text(line) -> str:
    """
    Join spans in a single PDF line into a string with inline Markdown markup.
    Consecutive spans with identical formatting are merged so we get
    **word1 word2** instead of **word1** **word2**.
    """
    # Normalize flags to only the bits we care about
    _FMT_MASK = _BOLD_BIT | _ITALIC_BIT | _MONO_BIT

    groups: list[tuple[list[str], int]] = []  # (words, norm_flags)
    for span in line["spans"]:
        raw = span["text"].strip()
        if not raw:
            continue
        norm = span["flags"] & _FMT_MASK
        if groups and groups[-1][1] == norm:
            groups[-1][0].append(raw)
        else:
            groups.append(([raw], norm))

    parts = []
    for word_list, norm in groups:
        parts.append(_decorate(" ".join(word_list), norm))

    return " ".join(parts)


def _block_to_segments(block):
    """
    Convert a text block to a list of (indent_x, text) segments.

    Lines within a block are usually word-wrapped continuations of the same
    bullet/paragraph, but they can also be separate bullet points that start
    with • or ▷.  We join continuation lines and split on explicit bullet chars.
    """
    page_height = None  # not needed here, footer filtered before

    segments = []
    pending_text = []
    pending_x = None

    for line in block["lines"]:
        line_text = _line_to_text(line).rstrip()
        if not line_text.strip():
            continue
        lx = line["bbox"][0]

        # Lines that start with an explicit bullet character are their own items
        stripped = line_text.lstrip()
        if stripped.startswith(("•", "▷", "–", "−", "—")):
            # Flush pending
            if pending_text:
                segments.append((pending_x, " ".join(pending_text)))
                pending_text = []
                pending_x = None
            segments.append((lx, line_text.strip()))
        elif pending_x is not None and abs(lx - pending_x) <= 20:
            # Continuation of the current segment (x is similar)
            pending_text.append(line_text.strip())
        else:
            # New segment: flush old, start new
            if pending_text:
                segments.append((pending_x, " ".join(pending_text)))
                pending_text = []
            pending_x = lx
            pending_text = [line_text.strip()]

    if pending_text:
        segments.append((pending_x, " ".join(pending_text)))

    return segments


def _is_footer_block(block, page_height: float) -> bool:
    return block["bbox"][1] > page_height - 15


def is_diagram_page(page) -> bool:
    text = page.get_text().strip()
    words = len(text.split())
    if words >= DIAGRAM_WORD_THRESHOLD:
        return False
    return len(page.get_drawings()) >= DIAGRAM_DRAW_MIN


def render_page_png(page, out_path: str, dpi: int = 150):
    page.get_pixmap(dpi=dpi).save(out_path)


def extract_content_images(page, page_num: int, img_dir: str, pdf_stem: str):
    results = []
    blocks = page.get_text("dict")["blocks"]
    img_idx = 0
    for block in blocks:
        if block["type"] != 1:
            continue
        bbox = block["bbox"]
        bw, bh = bbox[2] - bbox[0], bbox[3] - bbox[1]
        if bw < IMG_MIN_BBOX_SIDE or bh < IMG_MIN_BBOX_SIDE:
            continue
        if bh < 15 and bw > 150:  # thin horizontal separator
            continue
        try:
            xref = block.get("xref")
            if xref and xref > 0:
                img_data = page.parent.extract_image(xref)
                fname = f"{pdf_stem}_p{page_num:03d}_img{img_idx}.png"
                fpath = os.path.join(img_dir, fname)
                pix = fitz.Pixmap(img_data["image"])
                if pix.colorspace and pix.colorspace.n > 3:
                    pix = fitz.Pixmap(fitz.csRGB, pix)
                pix.save(fpath)
                results.append((bbox, fname))
                img_idx += 1
        except Exception:
            pass
    return results


def page_to_markdown(page, page_num: int, img_dir: str, pdf_stem: str) -> str:
    page_height = page.rect.height
    out = []

    # --- Diagram page ---
    if is_diagram_page(page):
        fname = f"{pdf_stem}_p{page_num:03d}_diagram.png"
        render_page_png(page, os.path.join(img_dir, fname))
        first_line = page.get_text().strip().split("\n")[0].strip()
        title = first_line if first_line else f"Slide {page_num + 1}"
        out.append(f"## {title}\n")
        out.append(f"![{title}](images/{fname})\n")
        return "\n".join(out)

    # --- Text page ---
    blocks = page.get_text("dict")["blocks"]
    text_blocks = [b for b in blocks if b["type"] == 0 and not _is_footer_block(b, page_height)]
    text_blocks.sort(key=lambda b: (round(b["bbox"][1] / 4) * 4, b["bbox"][0]))

    slide_title = None
    items: list[tuple[float, str]] = []  # (x, text)

    for block in text_blocks:
        # Skip blocks whose first line is purely the slide title size
        first_line_sizes = []
        for line in block["lines"]:
            sizes = [s["size"] for s in line["spans"] if s["text"].strip()]
            if sizes:
                first_line_sizes.append(sum(sizes) / len(sizes))
                break

        if first_line_sizes and first_line_sizes[0] >= 13:
            # This block contains the slide title
            title_text = " ".join(
                _line_to_text(ln).strip()
                for ln in block["lines"]
                if any(s["text"].strip() for s in ln["spans"])
            )
            if slide_title is None:
                slide_title = title_text.strip()
            else:
                # A second title-size block is a section divider on the slide
                items.append((block["bbox"][0], f"__H3__{title_text.strip()}"))
            continue

        if first_line_sizes and first_line_sizes[0] >= 11.5:
            # Sub-heading within a slide (e.g. "Frame problem (McCarthy & Hayes,1969)")
            subhead = " ".join(
                _line_to_text(ln).strip()
                for ln in block["lines"]
                if any(s["text"].strip() for s in ln["spans"])
            )
            items.append((block["bbox"][0], f"__H3__{subhead.strip()}"))
            continue

        # Check if block is a Beamer-style TOC entry: first line = tiny number,
        # second line = topic text (used in 'Overview' slides)
        lines = block["lines"]
        if len(lines) >= 2:
            first_sizes = [s["size"] for s in lines[0]["spans"] if s["text"].strip()]
            first_text = " ".join(s["text"] for s in lines[0]["spans"]).strip()
            if first_sizes and max(first_sizes) < 9 and re.fullmatch(r"\d+", first_text):
                # TOC entry: join number + subsequent text
                rest_text = " ".join(
                    _line_to_text(ln).strip() for ln in lines[1:]
                )
                items.append((block["bbox"][0], f"{first_text}. {rest_text.strip()}"))
                continue

        for seg_x, seg_text in _block_to_segments(block):
            items.append((seg_x, seg_text))

    # --- Build Markdown ---
    if slide_title:
        out.append(f"## {slide_title}\n")

    for x, text in items:
        if text.startswith("__H3__"):
            out.append(f"\n### {text[6:]}\n")
            continue

        stripped = text.lstrip()

        # Detect explicit bullet/arrow prefixes
        if stripped.startswith(("•", "·")):
            body = stripped[1:].strip()
            if x >= 40:
                out.append(f"  - {body}")
            else:
                out.append(f"- {body}")
            continue
        if stripped.startswith(("▷", "–", "−", "—")):
            body = stripped[1:].strip()
            out.append(f"  - {body}")
            continue

        # Detect numbered items: "N text" or "N. text"
        m = re.match(r"^(\d+)\.?\s+(.*)", stripped, re.DOTALL)
        if m and int(m.group(1)) <= 30:
            n, body = m.group(1), m.group(2)
            if x >= 40:
                out.append(f"  {n}. {body}")
            else:
                out.append(f"{n}. {body}")
            continue

        # Plain text — indent level from x position
        if x >= 50:
            out.append(f"    - {stripped}")
        elif x >= 30:
            out.append(f"  - {stripped}")
        else:
            out.append(f"- {stripped}")

    # --- Embedded raster images ---
    for _bbox, fname in extract_content_images(page, page_num, img_dir, pdf_stem):
        out.append(f"\n![image](images/{fname})\n")

    return "\n".join(out)


def convert_pdf(pdf_path: str, out_dir: str):
    pdf_stem = os.path.splitext(os.path.basename(pdf_path))[0]
    md_path = os.path.join(out_dir, f"{pdf_stem}.md")
    img_dir = os.path.join(out_dir, "images")
    os.makedirs(img_dir, exist_ok=True)

    doc = fitz.open(pdf_path)
    print(f"Converting {pdf_stem} ({len(doc)} pages)…")

    title = pdf_stem.replace("___", " – ").replace("_", " ")
    parts = [f"# {title}\n\n", f"> Source: `{os.path.basename(pdf_path)}`\n\n", "---\n\n"]

    for pg_num in range(len(doc)):
        md = page_to_markdown(doc[pg_num], pg_num, img_dir, pdf_stem)
        if md.strip():
            parts.append(md)
            parts.append("\n\n---\n\n")

    with open(md_path, "w", encoding="utf-8") as f:
        f.write("".join(parts))

    print(f"  → {md_path}")
    doc.close()


def main():
    wyklady_dir = os.path.dirname(os.path.abspath(__file__))
    handouts = sorted(f for f in os.listdir(wyklady_dir) if f.endswith("HANDOUT.pdf"))
    if not handouts:
        print("No HANDOUT PDFs found.")
        return
    for fname in handouts:
        convert_pdf(os.path.join(wyklady_dir, fname), wyklady_dir)
    print("\nDone.")


if __name__ == "__main__":
    main()
