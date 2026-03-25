# misc

Utility scripts for the `reprezentacje-wiedzy` course.

---

## `pdf_to_md.py` — Beamer PDF → Markdown

Converts Beamer-style lecture PDFs (slides) into readable Markdown files.
Each slide becomes an `##` section.  Diagram-heavy slides are rendered as
full-page PNGs and linked inline.

### Dependency

```bash
pip install pymupdf
```

### Quick usage

```bash
# Convert all *HANDOUT* PDFs in the docs/wyklady/ folder
python misc/pdf-to-md/pdf_to_md.py --dir docs/wyklady/pdf-handouts/ --pattern HANDOUT

# Convert every PDF in a directory, write output to a separate folder
python misc/pdf-to-md/pdf_to_md.py --dir docs/wyklady/pdf-handouts/ --out-dir output/

# Convert specific files
python misc/pdf-to-md/pdf_to_md.py docs/wyklady/pdf-handouts/RW_01___HANDOUT.pdf

# Convert all PDFs in the current working directory
cd docs/wyklady/pdf-handouts/
python ../../../misc/pdf-to-md/pdf_to_md.py
```

### Output

For each `<stem>.pdf` the script writes:

```
<out-dir>/
  <stem>.md          ← the Markdown file
  images/
    <stem>_p###_diagram.png   ← full-page render for diagram slides
    <stem>_p###_img#.png      ← embedded raster images (if any)
```

By default `<out-dir>` is the same directory as the source PDF.

### How it works

| Situation | What happens |
|---|---|
| Slide with ≥ 40 words | Text extracted and structured as Markdown |
| Slide with < 40 words + ≥ 10 vector paths | Rendered as PNG (diagram slide) |
| Font size ≥ 13 pt | `##` slide title |
| Font size 11.5–13 pt | `###` sub-heading within slide |
| x-indent < 30 pt | top-level `- bullet` |
| x-indent 30–50 pt | `  - sub-bullet` |
| x-indent ≥ 50 pt | `    - deep bullet` |
| Line starts with `•` or `▷` | bullet / sub-bullet respectively |
| Line matches `N text` (N ≤ 30) | numbered list item |
| Consecutive spans, same format | merged: `**word1 word2**` not `**word1** **word2**` |
| Footer (bottom 15 pt of page) | stripped |

### Tuning constants (top of script)

| Constant | Default | Effect |
|---|---|---|
| `DIAGRAM_WORD_THRESHOLD` | 40 | Slides with fewer words are candidates for PNG render |
| `DIAGRAM_DRAW_MIN` | 10 | Minimum vector paths to trigger PNG render |
| `TITLE_SIZE_MIN` | 13.0 pt | Font size for `##` slide title |
| `SUBHEAD_SIZE_MIN` | 11.5 pt | Font size for `###` sub-heading |
| `FOOTER_MARGIN` | 15 pt | Strip blocks this close to the page bottom |
| `IMG_MIN_BBOX_SIDE` | 15 pt | Ignore embedded images smaller than this |

Adjust these if a different Beamer theme uses different font sizes.
