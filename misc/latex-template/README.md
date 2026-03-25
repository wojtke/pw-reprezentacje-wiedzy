# LaTeX Template — Project Report

A4 document template with a title page, table of contents, and section stubs.

## Files

| File | Description |
|------|-------------|
| `template.tex` | Main source file |
| `politechnika.png` | Title page logo |
| `template.pdf` | Pre-rendered example |

## How to Compile

Requires `pdflatex` (included in BasicTeX on macOS, `texlive-base` on Linux).

```bash
pdflatex template.tex && pdflatex template.tex
```

Two passes are needed so the table of contents renders correctly.

Clean up build artifacts afterwards:

```bash
rm -f template.{aux,log,out,toc}
```

## Configuration

Edit the variables at the top of `template.tex`:

```latex
\newcommand{\coursename}{Nazwa przedmiotu}
\newcommand{\projecttitle}{Tytuł projektu}
\newcommand{\authorone}{Imię Nazwisko}
\newcommand{\authortwo}{Imię Nazwisko}
\newcommand{\projectdate}{Warszawa, \the\year}
```

## Optional Packages

The template ships with only core packages so it compiles with BasicTeX out of the box. Additional packages are listed as comments in the source. To enable them:

```bash
sudo tlmgr install titlesec enumitem subcaption booktabs float comment ragged2e
```

Then uncomment the relevant `\usepackage` lines in `template.tex`.
