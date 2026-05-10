# Checkpoint 3 — Design (v3, Go edition, high-level)

> Zakres tego dokumentu: cele PK3, decyzje techniczne, architektura i UX — na poziomie ogólnym.
> Specyfikację dziedziny, składnię języków i semantykę bierzemy z:
> - [`docs/Projekt_nr_4___2026.pdf`](../docs/Projekt_nr_4___2026.pdf) — treść projektu (Z1–Z7, Q1/Q2),
> - [`checkpoint2/checkpoint2.pdf`](../checkpoint2/checkpoint2.pdf) — sformalizowana teoria (do poprawienia wg uwag z PK2 przed PK4).
>
> **Zmiana względem v2:** stack technologiczny przeniesiony z Pythona na Go + Fyne (uzasadnienie w §1.2). Webowy frontend dodany jako **wariant zapasowy** (O7b) na wypadek problemów z Fyne — używa tego samego facade i dokładnie tego samego core'a; decyzja go/no-go do **5.05**. Architektura i podział modułów O1–O8 bez zmian merytorycznych — granice są zdefiniowane domenowo, nie językowo.

## 1. Cele i decyzje techniczne

### 1.1. Cele PK3 (12.05)

- **C1.** Aplikacja desktop GUI: edytor dziedziny + edytor kwerendy → odpowiedź TAK/NIE + krok-po-kroku trace procesu.
- **C2.** Pojedynczy `.exe` na Windows (statyczny binarny Go, ~15 MB), uruchamialny dwuklikiem na czystej maszynie bez instalacji jakiegokolwiek runtime'u.
- **C3.** Wbudowane przykłady z menu (kilka, dobrane tak, by łącznie pokryć Z1, Z3, Z5, Z6 i oba kwantyfikatory), każdy z 2–3 kwerendami i znanymi oczekiwanymi odpowiedziami; oczekiwane odpowiedzi pełnią rolę oracle'a w testach. Konkretny zestaw — do uzgodnienia w O6.
- **C4.** Core (parser + engine) jako pure-Go pakiet `internal/core` z zerową zależnością od UI — żeby alternatywny frontend (web) był nie tylko *możliwy*, ale **przygotowany jako oficjalny plan B (O7b)**.
- **C5.** Testy: pokrycie każdego modułu core'a + e2e na każdy przykład przez facade (golden-tests).

### 1.2. Decyzje techniczne

- **Go 1.22+** — single static binary, najczystsze spełnienie C2 (mniejszy plik, zero self-extract, zero runtime'u po stronie użytkownika). Trywialna cross-kompilacja `GOOS=windows GOARCH=amd64 go build` z macOS dla wariantu web (który nie używa cgo). Brak sum-typów w języku rekompensowany przez `BurntSushi/go-sumtype` (linter exhaustiveness) + `alecthomas/participle` (typed AST z gramatyką w tagach struktur).
- **Fyne v2** jako podstawowy frontend GUI. API w stylu Tkintera (kontrolki + container + handler), pełnoplatformowe (Mac dev, Windows produkcja), pojedynczy binarny statyczny. Świadomie akceptujemy, że wygląda nie-natywnie (custom-rendered OpenGL) — funkcjonalność > estetyka.
- **Webowy frontend zapasowy (O7b).** Drugi entrypoint `cmd/ds4-web/` serwuje SPA przez stdlib `net/http` na `localhost:<port>` i otwiera domyślną przeglądarkę. SPA (HTML + CSS + vanilla JS) wbudowane przez `//go:embed` — wciąż jeden `.exe`. **Nie wykorzystuje WebView2** — używa zewnętrznej przeglądarki preinstalowanej na 100% maszyn z Win10/11. Aktywujemy go, jeśli Fyne wykaże blokujący problem do **5.05** (tydzień przed PK3).
- **`alecthomas/participle/v2`** jako framework parsera. Świadoma zmiana względem v2 („ręczny recursive descent"): participle **nie generuje plików przed buildem** (działa przez reflection na strukturach), więc filozoficznie pozostaje blisko ręcznego parsera, ale pozwala wyrazić gramatykę DS4 tagami pól struktur — AST + parser w jednym, lokalizacja błędów (linia, kolumna) gratis.
- **`BurntSushi/go-sumtype`** jako linter (CI + IDE) — wymusza exhaustiveness `switch s := stmt.(type)` na wszystkich wariantach `Statement` / `Formula` / `Query`. Konfigurowany w `golangci-lint`. To jest mechanizm, który zastępuje kompilatorowy exhaustiveness check znany z C# / F#.
- **Pure-Go core (`internal/core`), bez importów `fyne.io/...` ani `net/http`** — wymóg z C4. Zarówno `cmd/ds4/` (Fyne), jak i `cmd/ds4-web/` (HTTP) konsumują **tylko `internal/api`**. Reguła `depguard` w `golangci-lint` tego pilnuje.
- **`go test` + `sebdah/goldie/v2`** — golden-testy `SolveResult` jako JSON; pierwsze uruchomienie z flagą `-update` zapisuje wzorce, kolejne porównują. Idealnie dopasowane do C5 + O6 (każdy przykład × każda kwerenda = jeden plik wzorcowy).
- **`stretchr/testify`** — assertions w unit testach core'a, uzupełnia `go test`.
- **Build workflow:**
  - **Dev na macOS:** `go run ./cmd/ds4` otwiera natywne okno Fyne (cgo+OpenGL działają na Macu z xcode-select).
  - **Finalny `.exe` (Fyne) budowany natywnie na Windowsie:** `go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4` — omija cgo + mingw cross-compile pain. Smoke-test artefaktu robimy na tej samej maszynie, jednym gestem.
  - **Wariant web (jeśli aktywowany):** `GOOS=windows GOARCH=amd64 go build -o ds4-web.exe ./cmd/ds4-web` — bez cgo, więc cross-compile z Maca działa bez Dockera.
- **Granica frontend ↔ core: `string` na wejściu, struktura JSON-serializowalna na wyjściu** — kontrakt jest dokładnie tym samym dla Fyne i web; web wariant marshaluje go przez `encoding/json` (tagi struct), Fyne renderuje bezpośrednio z pól.

## 2. Architektura

```
┌─────────────────────────────┬────────────────────────────────┐
│  PRIMARY: Fyne GUI          │  FALLBACK: Web (HTTP + SPA)    │
│  cmd/ds4/                   │  cmd/ds4-web/                  │
│  - widgets, layout          │  - net/http server             │
│  - tylko internal/api       │  - embed.FS ze static SPA      │
│                             │  - tylko internal/api          │
└──────────────┬──────────────┴──────────────┬─────────────────┘
               │                              │
               │   string → Solve() → SolveResult (JSON-able)
               │                              │
               └──────────────┬───────────────┘
                              ▼
       ┌─────────────────────────────────────────────────┐
       │ FACADE  (internal/api)                           │
       │   Solve · ValidateDomain · ListExamples · Load   │
       └──────────────────────┬──────────────────────────┘
                              │
       ┌──────────────────────▼──────────────────────────┐
       │ CORE  (internal/core, pure-Go)                   │
       │                                                   │
       │   ACTION LANGUAGE        │   QUERY LANGUAGE       │
       │   model · engine         │   composite · process  │
       │   formulas · Σ/Σ₀ · Res  │   Q1/Q2 × {nec, pos}   │
       │                                                   │
       │   PARSER (participle)                             │
       │   lexer · domain_parser · query_parser            │
       └─────────────────────────────────────────────────┘
```

**Layout repo:**

```
ds4/
├── go.mod
├── cmd/
│   ├── ds4/                    # Fyne entrypoint (primary)
│   │   └── main.go
│   └── ds4-web/                # HTTP server entrypoint (fallback)
│       ├── main.go
│       └── static/             # embedded via //go:embed
│           ├── index.html
│           ├── app.js
│           └── style.css
├── internal/
│   ├── api/                    # facade (O5)
│   │   ├── api.go              # Solve, ValidateDomain, ListExamples, LoadExample
│   │   └── result.go           # SolveResult, Step, Branch, Error
│   ├── core/
│   │   ├── model/              # O1 — Fluent, Action, State, Domain, Statement, Formula
│   │   ├── engine/             # O2, O3 — Σ, Σ₀, Res, query evaluation
│   │   └── parser/             # O4 — participle-based DomainParser, QueryParser
│   └── examples/               # O6 — //go:embed *.txt + manifest.json
└── tests/                      # e2e przez api facade (goldie)
```

**Zasady architektoniczne:**

1. **Core nie wie o UI.** Zero importów `fyne.io/...`, `net/http`, `embed` w `internal/core/`. Oba frontendy rozmawiają wyłącznie przez `internal/api`. Reguła `depguard` w `golangci-lint` tego pilnuje w CI.
2. **Action language ≠ query language.** Akcje złożone i procesy żyją wyłącznie po stronie query language — osobny pakiet, osobny parser, osobne testy.
3. **Składnia i semantyka biorą się z PK2.** Język akcji ma 8 rodzajów zdań (PK2 §3.2): `causes`, `releases`, `impossible`, `always`, `noninertial`, `initially`, `after`, `observable after`. Cukier syntaktyczny: `impossible A if π` ≡ `A causes ⊥ if π`, `initially α` ≡ `α after ε` — desugaring po stronie parsera (O4). **`noninertial f` *nie* jest cukrem** — to osobne zdanie strukturalne wyznaczające zbiór fluentów inercyjnych $\mathcal{F}_I$ używany przez `New` w O2 (PK2 §4.4); musi przeżyć w AST i być eksponowane przez `Domain`. Pozostałe zdania (`causes`, `releases`, `always`, `noninertial`, `after`, `observable after`) trzymamy w AST jako struct types implementujące marker interface `Statement` (z dyrektywą `//go-sumtype:decl Statement`). Konkretny kształt węzłów — decyzja O1+O4.
4. **Facade jest jedynym kontraktem dla frontendów (l.mn.).** `SolveResult` jest tym samym typem dla Fyne i web; web tylko marshaluje go przez `encoding/json` (tagi struct), Fyne renderuje bezpośrednio z pól. Każda zmiana w `internal/api` wymaga zaktualizowania obu frontendów (jeśli web jest aktywowany) — celowo trzymane w jednym pakiecie, żeby kompilator natychmiast pokazał wszystkie miejsca styku.
5. **Web fallback nie jest "drugim wymaganiem" — jest planem B.** Decyzja o aktywacji do 5.05; jeśli Fyne dowieziemy, `cmd/ds4-web/` nie wjeżdża do PK3 (kod może istnieć w repo, ale nie jest oddawanym artefaktem). Architektura jest ułożona tak, żeby przejście było mechaniczne (nowy `cmd/`, ten sam `internal/api`).

## 3. UX

### 3.1. Primary: Fyne (desktop)

```
┌──────────────────────────────────────────────────────────────────────┐
│ DS4 Reasoner                                              _ □ ✕      │
├──────────────────────────────────────────────────────────────────────┤
│ [Plik] [Przykłady] [Pomoc]                                            │
├──────────────────────────────────┬──────────────────────────────────┤
│  DZIEDZINA  (Entry)               │  KWERENDA  (Entry)   [▶ Oblicz] │
│                                   ├──────────────────────────────────┤
│                                   │  WYNIK  (Tree + Label)           │
├──────────────────────────────────┴──────────────────────────────────┤
│ ● Status · |Σ|, |Σ₀| · czas obliczenia · błędy walidacji              │
└──────────────────────────────────────────────────────────────────────┘
```

**Edytor dziedziny (lewa).** `widget.Entry` z multilinią na opis dziedziny.

**Edytor kwerendy + Oblicz (prawa góra).** `widget.Entry` na pojedynczą kwerendę i `widget.Button` uruchamiający `api.Solve()`.

**Panel wyniku (prawa dół).** Odpowiedź TAK/NIE/błąd jako `widget.Label`, drzewo trace'a jako `widget.Tree` (PK2 §8.7 — drzewo, nie zbiór), liczbowe podsumowanie (`|Σ|`, `|Σ₀|`); dla Q2 dodatkowo stany końcowe spełniające `γ`.

**Menu *Przykłady*.** `fyne.Menu` — jedno kliknięcie wypełnia oba edytory. Te same wejścia są oracle'em w testach e2e (C3, C5).

### 3.2. Fallback: Web (przeglądarka)

```
┌──────────────────────────────────────────────────────────────────────┐
│  http://localhost:8080                                                │
├──────────────────────────────────────────────────────────────────────┤
│ DS4 Reasoner             [Plik ▾] [Przykłady ▾] [Pomoc ▾]            │
├──────────────────────────────────┬──────────────────────────────────┤
│  DZIEDZINA  (textarea)            │  KWERENDA  (textarea) [▶ Oblicz] │
│                                   ├──────────────────────────────────┤
│                                   │  WYNIK  (<details>/<ul> tree)    │
├──────────────────────────────────┴──────────────────────────────────┤
│ ● Status · |Σ|, |Σ₀| · czas · błędy walidacji                         │
└──────────────────────────────────────────────────────────────────────┘
```

**User flow w wariancie web:**

1. Dwuklik `ds4-web.exe` → exe startuje lokalny serwer HTTP na pierwszym wolnym porcie z zakresu 8080–8090.
2. Otwiera default browser na `http://localhost:<port>`.
3. UI komunikuje się z backendem przez `fetch('/solve', { method: 'POST', body: ... })`.
4. Zamknięcie zakładki **nie** zamyka serwera — to robi okno konsoli (lub `Ctrl+C`).

Web wariant ma **identyczną semantykę** co Fyne — inny shell, ten sam kontrakt z O5.

## 4. Proponowany podział obszarów odpowiedzialności

### O1. Model i formuły (action language: dane)

**Co robi:** definiuje wszystkie typy danych domeny i logikę formuł zdaniowych. Wejście: nic (fundament). Wyjście: importowalne typy + funkcje na formułach (`internal/core/model`).

**Założenia:**
- Niezależny od parserów i od dynamiki — to czysta warstwa danych.
- AST pokrywa zdania PK2 §3.2 po desugaringu (`impossible`, `initially` znikają, reszta zostaje).
- `Statement`, `Formula` są marker interface'ami z dyrektywą `//go-sumtype:decl ...`; warianty (`Causes`, `Releases`, `Always`, `Noninertial`, `After`, `ObservableAfter`) jako struct types z metodą `statement()` / `formula()`. Ewaluatory używają `switch v := s.(type)` — exhaustiveness pilnowany przez `go-sumtype` w lincie.
- `Domain` eksponuje pochodne potrzebne dalej, m.in. zbiór fluentów inercyjnych $\mathcal{F}_I$ (z `noninertial`) — używany przez O2 w `New`.
- Formuły: AST + ewaluacja na stanie + **DNF** (PK2 §5.2 i §8.0) — wymagana przez O3 do wykrywania konfliktów.

**TODO:**
- `Fluent`, `Action`, `State`, `Domain`, `Statement`, `Formula` (warianty wg decyzji O1, zgodnie z punktem 3 z §2).
- AST formuł, ewaluacja na stanie, konwersja do DNF.
- Dyrektywy `//go-sumtype:decl` na wszystkich interface-typach wariantowych.

**Łączy się z:** O2 (eksportuje typy), O3 (eksportuje typy + ewaluację), O4 (parser produkuje `Domain`), O5 (facade konsumuje `Domain` i `State`).

### O2. Dynamika akcji prostej (action language: semantyka)

**Co robi:** dostarcza całą „fizykę" dziedziny dla pojedynczej akcji oraz buduje model na poziomie akcji prostych. Wejście: `Domain` z O1. Wyjście: `Σ`, `Σ₀` oraz `Res(action, state) → map[State]struct{}`.

**Założenia:**
- Operuje wyłącznie na akcjach prostych — akcje złożone i procesy są poza tym obszarem (O3).
- Σ, Σ₀ i `Res` zgodne z definicjami z PK2 §4 (łącznie z minimalizacją `New` przy fluentach inercyjnych $\mathcal{F}_I$ z O1, niedeterminizmem i priorytetem `impossible`); reszta systemu widzi tylko zbiór stanów.
- **Σ₀ należy do O2.** Filtracja Σ₀ przez `initially`, `after` i `observable after` (PK2 §4.2, §8.3) wymaga odwrotnego stosowania `Res` dla akcji prostych — czyli rzeczy, które już są w O2. O3 tej filtracji nie dotyka.
- Ramifikacje (Z6) z `always` są realizowane wyłącznie przez ograniczenie kandydatów w `Res₀` do Σ — nie ma osobnego mechanizmu „indirect effects".
- `State` musi być porównywalny i hashable (np. bitmapa fluentów jako `uint64` lub `string`), żeby `map[State]struct{}` działało jako set.

**TODO:**
- Generacja Σ z `always`; generacja Σ₀ z `initially` + `after` + `observable after`.
- `Res₀`, `New`, `Res` dla akcji prostej.

**Łączy się z:** O1 (typy + ewaluacja formuł), O3 (eksportuje `Res`, `Σ`, `Σ₀`), O5 (facade konsumuje `Σ`, `Σ₀` w wyniku).

### O3. Query language — akcje złożone, procesy, kwerendy

**Co robi:** odpowiada na kwerendy. Wejście: `Domain`, `Σ₀` i sparsowana kwerenda. Wyjście: `bool` + `*TraceNode` (drzewo) dla GUI/web.

**Założenia:**
- Korzysta z `Res` z O2, typów z O1 oraz **DNF z O1** (do wykrywania konfliktów wg PK2 §5.2). Sam nie buduje Σ ani Σ₀.
- Akcje złożone, dekompozycje i `Res(A,σ)` zgodnie z PK2 §5 (mechanizm AC: graf konfliktu + dekompozycje). Uwaga: warunek „rozłącznych fluentów" z treści projektu jest *słabszy* — nie wystarczy do PK4. Implementujemy AC.
- Semantyka kwantyfikatorów wg PK2 §7.3: `necessary` = ∀σ₀ ∀Ψ; `possibly` = ∃σ₀ ∃Ψ. Q1 patrzy na wykonalność procesu, Q2 dodatkowo na cel `γ`; `necessary γ after P` implikuje `necessary executable P`.
- Ewaluacja procesu jest **drzewem** (PK2 §8.7), nie zbiorem stanów osiągalnych — inaczej `necessary executable` policzy się błędnie. Trace ma kształt drzewa (`type TraceNode struct { ...; Children []*TraceNode }`) zakorzenionego w każdym σ₀ ∈ Σ₀, z gałęziami dla niedeterminizmu `Res` i z polem „powód niewykonalności" w węzłach blokowanych. Konkretny kształt — decyzja O3 z O5/O7.

**TODO:**
- Akcje złożone wg PK2 §5 (konflikty, dekompozycje, `Res` przez sumę po dekompozycjach).
- Procesy: iteracja `Ψ`, drzewo wykonania, agregacja stanów + trace.
- Q1, Q2 × {necessary, possibly} jako cztery funkcje ewaluujące drzewo + `γ`.

**Łączy się z:** O1 (typy), O2 (`Res`, `Σ`, `Σ₀`), O4 (dostaje sparsowaną kwerendę), O5 (zwraca trace + odpowiedź).

### O4. Parser — text → AST (participle-based)

**Co robi:** tłumaczy tekst dziedziny i tekst kwerendy na typy z O1/O3 przez **gramatykę zdefiniowaną tagami pól struktur** (`alecthomas/participle/v2`). Wejście: stringi. Wyjście: `Domain`, `Query`, lub strukturalny błąd z `lexer.Position`.

**Założenia:**
- Gramatyka żyje w samych typach jako tagi pól (\`@@\`, \`@Ident\`, alternatywy \`|\`, opcjonale \`?\`, listy \`*\` / \`+\`) — jedno źródło prawdy: definicja typu = definicja składni.
- Lokalizacja błędów (linia, kolumna) gratis — participle dostarcza `lexer.Position` w błędzie. Format błędu zgodny z `Error{Kind: "syntax", Location: {...}}` z O5.
- Identyfikatory typowane przez pozycję syntaktyczną w gramatyce (`Fluent`/`Action` mają osobne reguły leksera albo wspólny token `Ident` discriminowany kontekstem); O5 nie powtarza tej pracy.
- Desugaring `impossible`, `initially` — w `parser.normalize(rawDomain)` przed wydaniem `Domain` na zewnątrz pakietu.
- Świadoma decyzja: participle **nie generuje plików w czasie buildu** (nie ma `go generate` przed `go build`), więc filozoficznie pozostaje blisko ręcznego recursive descent — tylko bez boilerplate'u.

**TODO:**
- Definicja struktur participle dla domeny (8 zdań PK2 §3.2).
- Definicja struktur participle dla kwerendy (akcje złożone, procesy, Q1/Q2 × {necessary, possibly}).
- Pipeline: `participle.Build(...)` → `parser.Parse(text)` → `normalize()` (desugar) → `Domain` / `Query`.

**Łączy się z:** O1 (produkuje typy), O3 (produkuje `Query`), O5 (wywoływany z `Solve` / `ValidateDomain`), O7/O7b (przez format błędu).

### O5. Facade i testy e2e

**Co robi:** definiuje publiczny kontrakt API (`internal/api`) i jest jedynym punktem styku frontendów z resztą. Wejście: stringi. Wyjście: struct types z tagami `json:"..."` — JSON-serializowalne natywnie przez `encoding/json`.

**Założenia:**
- Facade orkiestruje O4 → O1/O2 → O3 i pakuje wynik w `SolveResult` (nie wycieka typami z core'a — wewnętrzny `Domain` nie wychodzi przez API).
- Testy e2e idą wyłącznie przez facade. Każdy przykład × każda kwerenda z O6 = jeden golden test (`goldie.AssertJson`).
- Kształt zwracanych structów żyje w `internal/api/result.go` (`SolveResult`, `Step`, `Branch`, `Error`, …). `SolveResult` musi unieść (a) odpowiedź TAK/NIE, (b) trace z O3, (c) dla Q2 — stany końcowe spełniające `γ`, (d) podsumowanie (`|Σ|`, `|Σ₀|`, czas) dla paska statusu w O7/O7b.
- Walidacja semantyczna na poziomie modelu (`Kind: "semantic"`): pusta Σ (sprzeczne `always`), pusta Σ₀ (sprzeczne `initially` / `after` / `observable after`). `Solve()` zatrzymuje się przed ewaluacją jeśli model jest pusty.
- Tagi `json:"..."` na polach `SolveResult` i podtypach — dla wariantu web `encoding/json` "po prostu działa"; dla Fyne tagi są ignorowane bez kosztu.

**TODO:**
- Implementacja `Solve`, `ValidateDomain`, `ListExamples`, `LoadExample` (te dwa ostatnie czytają z O6).
- Testy e2e z `goldie` konsumujące pliki + odpowiedzi z O6 i jadące przez facade.

**Łączy się z:** O4 + O1/O2 + O3 (wywołuje), O6 (konsumuje przykłady i oczekiwane odpowiedzi), O7 + O7b (jedyny kontrakt dla obu frontendów).

### O6. Oracle przykładów (ground truth)

**Co robi:** dostarcza wbudowane przykłady i ich poprawne odpowiedzi — jednocześnie zawartość menu *Przykłady* w GUI/web i oracle dla testów e2e (C3, C5).

**Założenia:**
- Wymaga starannego wyprowadzenia odpowiedzi z semantyki, nie kodowania.
- Każdy przykład = plik z dziedziną + 2–3 kwerendy + dla każdej kwerendy: oczekiwana odpowiedź TAK/NIE i krótka notatka „skąd" (jakie σ₀, którędy idzie trace).
- Łącznie przykłady mają pokrywać Z1, Z3, Z5, Z6, **Z7** (inercja, niedeterminizm, niewykonalność, ramifications, częściowy opis stanu początkowego — `|Σ₀| > 1`) oraz wszystkie cztery formy kwerend (`Q1` i `Q2`, każda w wariancie `necessary` i `possibly`).
- **Domyślny zestaw**: cztery przykłady wyprowadzone już w PK2 §10 (YSP z rosyjską ruletką, producent i konsumenci, dwa przełączniki, dwóch robotników).
- Pliki przykładów trzymane jako `//go:embed *.txt` w `internal/examples/`, z `manifest.json` opisującym `{id, title, file, queries: [{text, expected}]}` — wlatują do single-file `.exe` bez obsługi ścieżek na czystym Windowsie.

**Łączy się z:** O5 (oracle dla e2e + źródło dla `ListExamples`/`LoadExample`), O7 + O7b (zawartość menu *Przykłady*).

### O7. GUI Fyne (primary)

**Co robi:** desktopowa aplikacja Fyne — podstawowy frontend dostarczany w PK3. Wejście: `internal/api`. Wyjście: `cmd/ds4/ds4.exe`.

**Założenia:**
- Zero importów z `internal/core/` — GUI zna tylko `internal/api`. Reguła `depguard` w lincie tego pilnuje.
- Renderuje `SolveResult` i błędy wprost ze struct-a; cała logika decyzyjna („akcja niewykonalna w σ₃ bo X") pochodzi z O3 i jest tylko wyświetlana.
- Menu *Przykłady* zasilane przez `api.ListExamples()` / `api.LoadExample()`.
- Workflow: dev na macOS (`go run ./cmd/ds4`), finalny build natywnie na Windowsie (`go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4`).

**TODO:**
- Layout głównego okna z trzema strefami i paskiem statusu (`container.NewHSplit` + `container.NewVSplit`).
- Renderer `SolveResult` (odpowiedź jako Label, podsumowanie, trace per-krok jako `widget.Tree`, dla Q2 stany spełniające γ).
- Renderer błędów z lokalizacją (linia/kolumna) z O4.
- Podpięcie menu *Przykłady* do facade.

**Łączy się z:** O5 (jedyny kontrakt).

### O7b. Web frontend (fallback, conditional)

**Co robi:** Plan B — HTTP server + SPA, jeśli Fyne wykaże blokujący problem. Niesamodzielny: aktywujemy go decyzją do **5.05**. Wejście: `internal/api`. Wyjście: `cmd/ds4-web/ds4-web.exe`.

**Założenia:**
- Korzysta z **dokładnie tego samego `internal/api`** co Fyne — zero duplikacji logiki.
- SPA: HTML + CSS + vanilla JS, **żadnego frameworka frontendowego** (build step = anty-cel; `npm` = anty-cel). Statyczne assety embedowane przez `//go:embed cmd/ds4-web/static/*`.
- Stack: stdlib `net/http`, `encoding/json`. Zero zewnętrznych zależności runtime'owych poza tym, co już mamy.
- Endpointy:
  - `GET /` → `index.html`.
  - `GET /api/examples` → lista przykładów z manifestu (O6).
  - `GET /api/examples/{id}` → treść przykładu (dziedzina + lista kwerend).
  - `POST /api/solve` → body `{domain, query}` → `SolveResult` jako JSON.
  - `POST /api/validate` → body `{domain}` → `ValidationResult`.
- Autostart przeglądarki: po `ListenAndServe` na wolnym porcie z 8080–8090 wywołać OS-specific komendę (`open` na macOS dla testów dev, `cmd /c start` na Windowsie dla produkcji, `xdg-open` na Linuksie).
- Single binary nadal: cały SPA + serwer w jednym `.exe`.

**Trigger aktywacji (do 5.05):**
- Fyne nie kompiluje się stabilnie na Windowsie (np. cgo+OpenGL toolchain padł na maszynie do finalnego buildu), albo
- Fyne renderuje nieczytelnie na realnym przykładzie (np. trace tree nie mieści się / fonty nieczytelne / brak działającego rich textarea), albo
- Smoke test pokazuje crash/freeze przy realnym przykładzie z O6.

Jeśli żaden z trigerów się nie zrealizuje do 5.05 — `cmd/ds4-web/` zostaje w repo jako "preview", ale **nie wjeżdża jako artefakt PK3**. Do PK4 może być promowany do oficjalnego drugiego frontendu.

**TODO (warunkowo, po decyzji 5.05):**
- HTTP handlers w `cmd/ds4-web/main.go` opakowujące `internal/api` (cienkie — głównie marshal/unmarshal JSON).
- SPA: `index.html` + `app.js` + `style.css` w `cmd/ds4-web/static/`.
- Trace tree w SPA: zagnieżdżone `<details>`/`<ul>` (zero zależności frontendowych).
- Browser autostart cross-platform (helper z `runtime.GOOS` switchem).
- Krótkie README dla użytkownika końcowego: „uruchom `ds4-web.exe`, otworzy się przeglądarka, nie zamykaj okna konsoli".

**Łączy się z:** O5 (jedyny kontrakt), O6 (assety przez API).

### O8. Pakowanie + Windows-build

**Co robi:** zamienia źródło Go w pojedynczy `.exe` uruchamialny dwuklikiem na czystym Windowsie (C2). Wejście: drzewo źródeł. Wyjście: artefakt dystrybucji + dowód, że działa.

**Założenia:**
- **Primary (Fyne):** build **natywnie na Windowsie** — `go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4`. Świadomie omijamy cross-compile z Maca (cgo + OpenGL + mingw to udręka).
- **Fallback (web):** build na dowolnej platformie — `GOOS=windows GOARCH=amd64 go build -ldflags="-s -w" -o ds4-web.exe ./cmd/ds4-web`. Brak cgo, więc cross-compile z Maca działa bez Dockera.
- Single binary statyczny w obu wariantach, ~15 MB, brak runtime'u u użytkownika.
- Pierwszy artefakt: „pusty exe z zaślepką GUI" działający na czystym Windowsie — weryfikacja toolchainu przed finałowym pakowaniem.
- **Smoke-test artefaktu na fizycznym czystym Windowsie obowiązkowy** przed PK3 (laptop kolegi z zespołu / lab PW).

**TODO:**
- `Makefile` lub `taskfile.yml` z celami `dev`, `build-fyne` (na Windows), `build-web` (cross-compile), `test`, `lint`.
- Pierwszy `hello.exe` ze szkieletem z O7 — smoke-test toolchainu na czystym Windowsie.
- Konfiguracja `golangci-lint` z włączonym `depguard` (architecture rule: `internal/core` nie importuje `fyne.io/...` ani `net/http`) i `gosumtype`.
- Finalny build + test na czystym Windowsie.

**Łączy się z:** O7 (pakuje primary), O7b (pakuje fallback jeśli aktywowany), O5 (pakuje pakiet `internal/api`), reszta — pośrednio.

## 5. Otwarte kwestie / do uzgodnienia

Drobiazgi, które trzeba rozstrzygnąć, ale nie blokują startu prac. Każdy punkt wymaga 15-minutowej decyzji właścicieli wymienionych w nawiasie.

- **Kształt `TraceNode` i `Query` AST** (O3 + O4 + O5 + O7/O7b). Trace musi być drzewem (patrz O3) i unieść świadków γ dla Q2; konkretne pola structa i shape `Query` — domówić zanim O3 i O7 napiszą cokolwiek niezmiennikowego.
- **`SolveResult` — pola statusu** (O5 + O7/O7b). `|Σ|`, `|Σ₀|`, czas obliczenia, świadki γ — albo wchodzą do `SolveResult`, albo znikają z UX. Wybrać.
- **Zachowanie `Solve()` przy pustym modelu** (O5 + O7/O7b). Kiedy Σ = ∅ lub Σ₀ = ∅: błąd `Kind: "semantic"` z komunikatem, czy odpowiedź vacuous-true/false dla każdej kwerendy? Pinujemy konwencję, GUI ją renderuje.
- **Lexer dla DS4 w participle** (O4). Wbudowany `lexer.Default` czy custom rules? Słowa kluczowe (`causes`, `releases`, `if`, `always`, ...) — case-sensitive czy insensitive? Identyfikatory — czy `Fluent` i `Action` to wspólny `Ident` discriminowany syntaktycznie, czy mają oddzielne klasy tokenów?
- **Format pliku przykładów** (O6 + O5 + O8). `internal/examples/` ma `.txt` z dziedziną + `.queries.json` z kwerendami i oczekiwanymi odpowiedziami obok? Czy jeden plik `.json` na przykład z polami `domain`, `queries`? Decyzja wpływa na `manifest.json`.
- **GUI a wiele kwerend per przykład** (O7 + O7b + O6). Po `LoadExample` przykład ma 2–3 kwerendy; menu pokazuje listę wyboru, dropdown czy podmenu — drobny UX, ale O7 musi wiedzieć. Web wariant prawdopodobnie pokaże dropdown nad polem kwerendy.
- **Decyzja go/no-go dla O7b** (O7 + O7b + O8 + cały zespół). Termin: **5.05**. Kryteria w O7b. Domyślnie: **no** (Fyne wystarcza), web zostaje w repo jako preview do PK4.
- **Web port + autostart przeglądarki** (O7b). Stały port (8080)? Pierwszy wolny z zakresu? Jak otwieramy default browser na Windowsie (`cmd /c start ""`, `rundll32 url.dll`)?
- **Antivirus false-positives** (O8). Statyczne Go-binaries bywają fałszywie wykrywane przez Defender SmartScreen. Czy chcemy podpisać `.exe` (potrzebny cert), czy oddajemy niepodpisany z notką w README?