#!/usr/bin/env python3
"""Render PRESENTATION.md (including mermaid diagrams) to PRESENTATION.pdf via headless Chrome."""
from __future__ import annotations
import json
import shutil
import subprocess
import sys
import tempfile
import urllib.parse
from pathlib import Path

HERE = Path(__file__).resolve().parent
MD = HERE / "PRESENTATION.md"
PDF = HERE / "PRESENTATION.pdf"
TMP_HTML = HERE / "_presentation_render.html"

CHROME = Path("/Applications/Google Chrome.app/Contents/MacOS/Google Chrome")

if not MD.exists():
    sys.exit(f"missing: {MD}")
if not CHROME.exists():
    sys.exit(f"Chrome not found at {CHROME}")

md_text = MD.read_text(encoding="utf-8")
md_json = json.dumps(md_text)

html = f"""<!doctype html>
<html lang="pl">
<head>
<meta charset="utf-8">
<title>RENDERING</title>
<script src="https://cdn.jsdelivr.net/npm/marked/marked.min.js"></script>
<style>
  body {{ font-family: -apple-system, system-ui, "Helvetica Neue", Arial, sans-serif;
         max-width: 900px; margin: 24px auto; padding: 0 24px; color: #222; line-height: 1.5; }}
  h1, h2, h3 {{ color: #111; }}
  h1 {{ font-size: 26px; }}
  h2 {{ font-size: 20px; border-bottom: 1px solid #eee; padding-bottom: 4px; margin-top: 32px; }}
  h3 {{ font-size: 16px; }}
  p, li {{ font-size: 13px; }}
  code {{ background: #f4f4f4; padding: 1px 4px; border-radius: 3px; font-size: 12px; }}
  pre {{ background: #f7f7f7; padding: 10px; border-radius: 6px; overflow-x: auto; font-size: 12px; }}
  pre code {{ background: transparent; padding: 0; }}
  .mermaid {{ background: #fff; padding: 8px; text-align: center; }}
  .mermaid svg {{ max-width: 100%; height: auto; }}
  img {{ max-width: 100%; }}
  blockquote {{ border-left: 4px solid #ddd; margin: 0; padding: 4px 12px; color: #555; }}
  hr {{ border: 0; border-top: 1px solid #ddd; margin: 24px 0; }}
  @page {{ size: A4; margin: 14mm 14mm; }}
  /* avoid breaking mermaid figures across pages */
  .mermaid, pre, img {{ page-break-inside: avoid; break-inside: avoid; }}
  h1, h2, h3 {{ page-break-after: avoid; break-after: avoid; }}
</style>
</head>
<body>
<div id="content"></div>
<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({{ startOnLoad: false, theme: 'default', securityLevel: 'loose', flowchart: {{ htmlLabels: true }} }});

  const md = {md_json};
  const renderer = new marked.Renderer();
  const escape = s => s.replace(/[&<>]/g, c => ({{'&':'&amp;','<':'&lt;','>':'&gt;'}}[c]));
  renderer.code = (code, lang) => {{
    if (lang === 'mermaid') return '<pre class="mermaid">' + code + '</pre>';
    return '<pre><code class="language-' + (lang || '') + '">' + escape(code) + '</code></pre>';
  }};
  marked.setOptions({{ renderer, gfm: true, breaks: false }});
  document.getElementById('content').innerHTML = marked.parse(md);

  try {{
    await mermaid.run({{ querySelector: '.mermaid' }});
  }} catch (e) {{
    document.body.insertAdjacentHTML('afterbegin', '<pre style="color:red">mermaid error: ' + e.message + '</pre>');
  }}
  // Signal to headless Chrome that we are done.
  document.title = 'READY';
</script>
</body>
</html>
"""

TMP_HTML.write_text(html, encoding="utf-8")

file_url = "file://" + urllib.parse.quote(str(TMP_HTML))

with tempfile.TemporaryDirectory() as udir:
    cmd = [
        str(CHROME),
        "--headless=new",
        "--disable-gpu",
        "--no-sandbox",
        f"--user-data-dir={udir}",
        "--virtual-time-budget=15000",
        "--run-all-compositor-stages-before-draw",
        "--no-pdf-header-footer",
        f"--print-to-pdf={PDF}",
        file_url,
    ]
    print("running:", " ".join(cmd))
    result = subprocess.run(cmd, capture_output=True, text=True)
    print(result.stdout)
    print(result.stderr, file=sys.stderr)
    if result.returncode != 0:
        sys.exit(result.returncode)

if PDF.exists() and PDF.stat().st_size > 0:
    print(f"OK: {PDF} ({PDF.stat().st_size:,} bytes)")
else:
    sys.exit("PDF was not produced.")
