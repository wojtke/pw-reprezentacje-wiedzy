# Checkpoint 3 — Design (TL;DR)

## 1. Cele PK3

- **C1.** Desktop GUI: edytor dziedziny + edytor kwerendy → TAK/NIE + trace.
- **C2.** Pojedynczy `.exe` na Windows (~15 MB), bez instalacji runtime'u.
- **C3.** Wbudowane przykłady z menu; oczekiwane odpowiedzi służą jako oracle w testach.
- **C4.** Core (parser + engine) jako pure-Go `internal/core`, zero zależności od UI.
- **C5.** Testy: unit per moduł core'a + e2e przez facade (golden-tests).

## 2. Stack

- **Go 1.22+** — single static binary.
- **Fyne v2** — primary GUI.
- **Web fallback** (`cmd/ds4-web/`) — HTTP + SPA (vanilla JS) jako plan B
- **`alecthomas/participle/v2`** — biblioteka do parsowania
- **`BurntSushi/go-sumtype`** — exhaustiveness check
- **`go test` + `sebdah/goldie/v2`** — do testów
- **`stretchr/testify`** — czytelne assertions

**Build:**
- Dev na macOS: `go run ./cmd/ds4`.
- Final `.exe` (Fyne): natywnie na Windowsie.
- Web wariant: cross-compile z Maca (bez cgo).

## 3. Architektura

```
┌─────────────────────────────┐    ┌────────────────────────────────┐
│  PRIMARY: Fyne GUI          │    │  FALLBACK: Web (HTTP + SPA)    │
│  cmd/ds4/                   │    │  cmd/ds4-web/                  │
└──────────────┬──────────────┘    └────────────────┬───────────────┘
               │                                    │
               │   (ctx, string) → Solve() → SolveResult (JSON-able)
               │                                    │
               └─────────────────┬──────────────────┘
                                 ▼
         ┌─────────────────────────────────────────────────┐
         │ FACADE  (internal/api)                          │
         │   Solve · ValidateDomain · ListExamples · Load  │
         └──────────────────────┬──────────────────────────┘
                                ▼
         ┌─────────────────────────────────────────────────┐
         │ CORE  (internal/core, pure-Go)                  │
         │   model · engine · parser (participle)          │
         └─────────────────────────────────────────────────┘
```

**Layout repo:**

```
ds4/
├── cmd/
│   ├── ds4/            # Fyne entrypoint
│   └── ds4-web/        # HTTP entrypoint (fallback)
│       └── static/     # SPA via //go:embed
├── internal/
│   ├── api/            # facade
│   ├── core/
│   │   ├── model/      # Fluent, Action, State, Domain, Statement, Formula
│   │   ├── engine/     # Σ, Σ₀, Res, query evaluation
│   │   └── parser/     # participle-based
│   └── examples/       # //go:embed *.txt + manifest.json
└── tests/              # e2e przez facade (goldie)
```

## 3. UX

**Fyne (primary):**

```
┌──────────────────────────────────────────────────────────────────────┐
│ DS4 Reasoner                                                          │
├──────────────────────────────────────────────────────────────────────┤
│ [Plik] [Przykłady] [Pomoc]                                            │
├──────────────────────────────────┬───────────────────────────────────┤
│  DZIEDZINA  (Entry)              │  KWERENDA (Entry)   [▶ Oblicz]    │
│                                  ├───────────────────────────────────┤
│                                  │  WYNIK  (Tree + Label)            │
├──────────────────────────────────┴───────────────────────────────────┤
│ ● Status · |Σ|, |Σ₀| · czas · błędy walidacji                         │
└──────────────────────────────────────────────────────────────────────┘
```

- Lewa: edytor dziedziny (`widget.Entry` multilinia).
- Prawa góra: edytor kwerendy + przycisk `Oblicz` → `api.Solve(ctx, ...)` (gorutyna + przycisk „Anuluj").
- Prawa dół: TAK/NIE/błąd (`Label`), trace jako `widget.Tree`, podsumowanie `|Σ|, |Σ₀|`; dla Q2 — stany spełniające γ.
- Menu *Przykłady*: jedno kliknięcie wypełnia oba edytory.

**Web (fallback):** to samo UI w przeglądarce (textarea + `<details>/<ul>` na trace), `fetch('/api/solve')`. Dwuklik `ds4-web.exe` → serwer na pierwszym wolnym porcie 8080–8090 → otwarcie default browsera. Identyczna semantyka co Fyne.
