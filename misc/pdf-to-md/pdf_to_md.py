#!/usr/bin/env python3
"""
pdf_to_md.py — Convert Beamer-style lecture PDFs to Markdown.

For each input PDF the script produces:
  - <stem>.md          next to the PDF (or in --out-dir if given)
  - images/<stem>_p###_diagram.png   for diagram-heavy slides
  - images/<stem>_p###_img#.png      for embedded raster images

Tuned for Warsaw University of Technology / MiNI Beamer theme but should work
for any Beamer PDF with standard font-size hierarchy.
"""

import argparse
import os
import re
import sys

import fitz  # pip install pymupdf

# ---------------------------------------------------------------------------
# Tunables
# ---------------------------------------------------------------------------

# Pages with fewer words AND >= DIAGRAM_DRAW_MIN vector paths are rendered
# as a full-page PNG instead of being parsed as text.
DIAGRAM_WORD_THRESHOLD = 40
DIAGRAM_DRAW_MIN = 10

# Embedded raster images whose bounding box is smaller than this (in PDF pts)
# on either side are assumed decorative (bullets, borders) and skipped.
IMG_MIN_BBOX_SIDE = 15

# Font-size thresholds (PDF pts) for heading detection.
# Slide titles are typically ~14 pt; sub-headings ~12 pt; body ~11 pt.
TITLE_SIZE_MIN = 13.0     # → ## (slide title / h2)
SUBHEAD_SIZE_MIN = 11.5   # → ### (named section within slide)

# Footer blocks are at the very bottom of every Beamer slide.
FOOTER_MARGIN = 15  # pts from the bottom edge

# ---------------------------------------------------------------------------
# PyMuPDF span-flag bits
# ---------------------------------------------------------------------------
_ITALIC_BIT = 2
_BOLD_BIT = 16
_MONO_BIT = 8
_FMT_MASK = _BOLD_BIT | _ITALIC_BIT | _MONO_BIT


# ---------------------------------------------------------------------------
# Inline text helpers
# ---------------------------------------------------------------------------

def _decorate(text: str, flags: int) -> str:
    """Wrap text with Markdown bold/italic/code markers based on font flags."""
    if not text.strip():
        return text
    bold = bool(flags & _BOLD_BIT)
    italic = bool(flags & _ITALIC_BIT)
    mono = bool(flags & _MONO_BIT)
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
    Join all spans on a single PDF line into a Markdown string.
    Consecutive spans that share the same formatting are merged so that
    **word1 word2** is emitted instead of **word1** **word2**.
    """
    groups: list[tuple[list[str], int]] = []
    for span in line["spans"]:
        raw = span["text"].strip()
        if not raw:
            continue
        norm = span["flags"] & _FMT_MASK
        if groups and groups[-1][1] == norm:
            groups[-1][0].append(raw)
        else:
            groups.append(([raw], norm))

    return " ".join(_decorate(" ".join(words), norm) for words, norm in groups)


# ---------------------------------------------------------------------------
# Block → segment conversion
# ---------------------------------------------------------------------------

def _block_to_segments(block) -> list[tuple[float, str]]:
    """
    Convert one PDF text block into a list of (indent_x, text) segments.

    Lines that start with an explicit bullet glyph (•, ▷, –, …) become their
    own segments.  All other lines at a similar x-offset are treated as
    word-wrapped continuations of the same paragraph and are joined.
    """
    segments: list[tuple[float, str]] = []
    pending: list[str] = []
    pending_x: float | None = None

    for line in block["lines"]:
        text = _line_to_text(line).rstrip()
        if not text.strip():
            continue
        lx = line["bbox"][0]
        stripped = text.lstrip()

        if stripped.startswith(("•", "·", "▷", "–", "−", "—")):
            if pending:
                segments.append((pending_x, " ".join(pending)))
                pending, pending_x = [], None
            segments.append((lx, stripped))
        elif pending_x is not None and abs(lx - pending_x) <= 20:
            pending.append(stripped)
        else:
            if pending:
                segments.append((pending_x, " ".join(pending)))
            pending, pending_x = [stripped], lx

    if pending:
        segments.append((pending_x, " ".join(pending)))

    return segments


# ---------------------------------------------------------------------------
# Page-level helpers
# ---------------------------------------------------------------------------

def _is_footer_block(block, page_height: float) -> bool:
    return block["bbox"][1] > page_height - FOOTER_MARGIN


def _is_diagram_page(page) -> bool:
    words = len(page.get_text().split())
    if words >= DIAGRAM_WORD_THRESHOLD:
        return False
    return len(page.get_drawings()) >= DIAGRAM_DRAW_MIN


def _render_png(page, path: str, dpi: int = 150) -> None:
    page.get_pixmap(dpi=dpi).save(path)


def _extract_raster_images(
    page, page_num: int, img_dir: str, pdf_stem: str
) -> list[tuple[tuple, str]]:
    """Return list of (bbox, filename) for non-decorative embedded images."""
    results = []
    idx = 0
    for block in page.get_text("dict")["blocks"]:
        if block["type"] != 1:
            continue
        bbox = block["bbox"]
        bw, bh = bbox[2] - bbox[0], bbox[3] - bbox[1]
        if bw < IMG_MIN_BBOX_SIDE or bh < IMG_MIN_BBOX_SIDE:
            continue
        if bh < 15 and bw > 150:   # thin horizontal separator line
            continue
        try:
            xref = block.get("xref")
            if xref and xref > 0:
                img_data = page.parent.extract_image(xref)
                fname = f"{pdf_stem}_p{page_num:03d}_img{idx}.png"
                pix = fitz.Pixmap(img_data["image"])
                if pix.colorspace and pix.colorspace.n > 3:
                    pix = fitz.Pixmap(fitz.csRGB, pix)
                pix.save(os.path.join(img_dir, fname))
                results.append((bbox, fname))
                idx += 1
        except Exception:
            pass
    return results


# ---------------------------------------------------------------------------
# Per-page conversion
# ---------------------------------------------------------------------------

def _page_to_markdown(page, page_num: int, img_dir: str, pdf_stem: str) -> str:
    page_height = page.rect.height
    out: list[str] = []

    # -- Diagram slide: render full page as PNG --------------------------------
    if _is_diagram_page(page):
        fname = f"{pdf_stem}_p{page_num:03d}_diagram.png"
        _render_png(page, os.path.join(img_dir, fname))
        first_line = page.get_text().strip().split("\n")[0].strip()
        title = first_line or f"Slide {page_num + 1}"
        out.append(f"## {title}\n")
        out.append(f"![{title}](images/{fname})\n")
        return "\n".join(out)

    # -- Text slide ------------------------------------------------------------
    blocks = [
        b for b in page.get_text("dict")["blocks"]
        if b["type"] == 0 and not _is_footer_block(b, page_height)
    ]
    blocks.sort(key=lambda b: (round(b["bbox"][1] / 4) * 4, b["bbox"][0]))

    slide_title: str | None = None
    items: list[tuple[float, str]] = []   # (x_offset, text)

    for block in blocks:
        # Measure the average font size of the block's first line
        first_sizes: list[float] = []
        for line in block["lines"]:
            sizes = [s["size"] for s in line["spans"] if s["text"].strip()]
            if sizes:
                first_sizes.append(sum(sizes) / len(sizes))
                break

        # Slide title
        if first_sizes and first_sizes[0] >= TITLE_SIZE_MIN:
            text = " ".join(
                _line_to_text(ln).strip()
                for ln in block["lines"]
                if any(s["text"].strip() for s in ln["spans"])
            )
            if slide_title is None:
                slide_title = text.strip()
            else:
                items.append((block["bbox"][0], f"__H3__{text.strip()}"))
            continue

        # Sub-heading (named section within a slide)
        if first_sizes and first_sizes[0] >= SUBHEAD_SIZE_MIN:
            text = " ".join(
                _line_to_text(ln).strip()
                for ln in block["lines"]
                if any(s["text"].strip() for s in ln["spans"])
            )
            items.append((block["bbox"][0], f"__H3__{text.strip()}"))
            continue

        # Beamer TOC entry: first line is a tiny standalone digit, rest is the topic
        lines = block["lines"]
        if len(lines) >= 2:
            sizes0 = [s["size"] for s in lines[0]["spans"] if s["text"].strip()]
            text0 = " ".join(s["text"] for s in lines[0]["spans"]).strip()
            if sizes0 and max(sizes0) < 9 and re.fullmatch(r"\d+", text0):
                rest = " ".join(_line_to_text(ln).strip() for ln in lines[1:])
                items.append((block["bbox"][0], f"{text0}. {rest.strip()}"))
                continue

        for seg_x, seg_text in _block_to_segments(block):
            items.append((seg_x, seg_text))

    # -- Render items as Markdown ----------------------------------------------
    if slide_title:
        out.append(f"## {slide_title}\n")

    for x, text in items:
        if text.startswith("__H3__"):
            out.append(f"\n### {text[6:]}\n")
            continue

        s = text.lstrip()

        if s.startswith(("•", "·")):
            body = s[1:].strip()
            out.append(f"  - {body}" if x >= 40 else f"- {body}")
            continue
        if s.startswith(("▷", "–", "−", "—")):
            out.append(f"  - {s[1:].strip()}")
            continue

        m = re.match(r"^(\d+)\.?\s+(.*)", s, re.DOTALL)
        if m and int(m.group(1)) <= 30:
            prefix = f"  {m.group(1)}." if x >= 40 else f"{m.group(1)}."
            out.append(f"{prefix} {m.group(2)}")
            continue

        if x >= 50:
            out.append(f"    - {s}")
        elif x >= 30:
            out.append(f"  - {s}")
        else:
            out.append(f"- {s}")

    # -- Embedded raster images ------------------------------------------------
    for _bbox, fname in _extract_raster_images(page, page_num, img_dir, pdf_stem):
        out.append(f"\n![image](images/{fname})\n")

    return "\n".join(out)


# ---------------------------------------------------------------------------
# Top-level conversion
# ---------------------------------------------------------------------------

def convert_pdf(pdf_path: str, out_dir: str) -> None:
    pdf_stem = os.path.splitext(os.path.basename(pdf_path))[0]
    md_path = os.path.join(out_dir, f"{pdf_stem}.md")
    img_dir = os.path.join(out_dir, "images")
    os.makedirs(img_dir, exist_ok=True)

    doc = fitz.open(pdf_path)
    print(f"  {pdf_stem} ({len(doc)} pages) …", end=" ", flush=True)

    title = pdf_stem.replace("___", " – ").replace("_", " ")
    parts = [
        f"# {title}\n\n",
        f"> Source: `{os.path.basename(pdf_path)}`\n\n",
        "---\n\n",
    ]

    for pg in range(len(doc)):
        md = _page_to_markdown(doc[pg], pg, img_dir, pdf_stem)
        if md.strip():
            parts.append(md)
            parts.append("\n\n---\n\n")

    with open(md_path, "w", encoding="utf-8") as f:
        f.write("".join(parts))

    doc.close()
    print(f"→ {md_path}")


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def _parse_args() -> argparse.Namespace:
    p = argparse.ArgumentParser(
        description="Convert Beamer lecture PDFs to Markdown.",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
examples:
  # convert all *HANDOUT* PDFs in the current directory
  python pdf_to_md.py --pattern HANDOUT

  # convert specific files
  python pdf_to_md.py RW_01___HANDOUT.pdf RW_02___HANDOUT.pdf

  # convert all PDFs in a directory, write output elsewhere
  python pdf_to_md.py --dir /path/to/pdfs --out-dir /path/to/output
        """,
    )
    p.add_argument(
        "files",
        nargs="*",
        metavar="PDF",
        help="PDF files to convert (ignored when --dir is given).",
    )
    p.add_argument(
        "--dir",
        metavar="DIR",
        help="Directory to scan for PDFs (default: current directory).",
    )
    p.add_argument(
        "--pattern",
        metavar="STR",
        default="",
        help="Only process PDFs whose filename contains STR (case-sensitive).",
    )
    p.add_argument(
        "--out-dir",
        metavar="DIR",
        help=(
            "Write .md files here instead of next to each PDF. "
            "images/ sub-directory is created inside out-dir."
        ),
    )
    return p.parse_args()


def main() -> None:
    args = _parse_args()

    # Collect input files
    if args.dir:
        scan_dir = os.path.abspath(args.dir)
        pdf_files = sorted(
            os.path.join(scan_dir, f)
            for f in os.listdir(scan_dir)
            if f.lower().endswith(".pdf") and args.pattern in f
        )
    elif args.files:
        pdf_files = [os.path.abspath(f) for f in args.files]
    else:
        # Default: all PDFs matching --pattern in current directory
        scan_dir = os.getcwd()
        pdf_files = sorted(
            os.path.join(scan_dir, f)
            for f in os.listdir(scan_dir)
            if f.lower().endswith(".pdf") and args.pattern in f
        )

    if not pdf_files:
        print("No PDFs found. Use --help for usage.")
        sys.exit(1)

    print(f"Converting {len(pdf_files)} file(s):")
    for pdf_path in pdf_files:
        out_dir = os.path.abspath(args.out_dir) if args.out_dir else os.path.dirname(pdf_path)
        convert_pdf(pdf_path, out_dir)

    print("\nDone.")


if __name__ == "__main__":
    main()
