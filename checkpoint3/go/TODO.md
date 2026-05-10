# TODO — Checkpoint 3

## Przypisania

> Wpisz swój nick / imię w kolumnie *Owner*. Można dopisać kilka osób przecinkiem.

| ID  | Obszar                                            | Owner |
| --- | ------------------------------------------------- | ----- |
| O1  | Model i formuły (action language: dane)           |       |
| O2  | Dynamika akcji prostej (Σ, Σ₀, Res)               |       |
| O3  | Query language (akcje złożone, procesy, kwerendy) |       |
| O4  | Parser (text → AST, participle)                   |       |
| O5  | Facade i testy e2e                                |       |
| O6  | Oracle przykładów (ground truth)                  |       |
| O7  | GUI Fyne (primary)                                |       |
| O7b | Web frontend (fallback, conditional)              |       |
| O8  | Pakowanie + Windows-build                         |       |

## Obszary odpowiedzialności

> Każdy obszar ma być wykonywalny **niezależnie** dzięki kontraktom typowym z sąsiadami: typy domeny z O1 (`internal/core/model`), kształt `SolveResult` z O5 (`internal/api/result.go`), `Query` AST z O3 (`internal/core/engine/query.go`).

### O1. Model i formuły (action language: dane)

**TL;DR:** Zdefiniuj typy AST dziedziny (`Fluent`, `Action`, `State`, `Domain`, `Statement`, `Formula`) i logikę formuł (ewaluacja na stanie + konwersja do DNF).

**Estimate:** M.

**Wejście:** nic — warstwa fundamentalna.
**Wyjście:** publiczne typy + funkcje na formułach.

**Lokalizacja:** `internal/core/model/`.

**Dependencje:** brak. Eksportuje typy do O2, O3, O4, O5.

**Założenia:**
- Czysta warstwa danych — zero dynamiki, zero parsowania.
- AST już *po* desugaringu (`impossible`/`initially` zniknęły w O4 — tutaj ich nie ma).
- `Statement` i `Formula` to marker interface'y z dyrektywą `//go-sumtype:decl`. Warianty `Statement`: `Causes`, `Releases`, `Always`, `Noninertial`, `After`, `ObservableAfter` jako struct types.
- `State` musi być porównywalny i hashable (np. `uint64` bitmapa fluentów lub `string`) — używany jako klucz mapy w O2.
- `Domain` eksponuje pochodne — m.in. zbiór fluentów inercyjnych $\mathcal{F}_I$ (z `noninertial`), używany przez O2 w `New`.
- Konwersja formuły do **DNF** (PK2 §5.2, §8.0) — wymagana przez O3 do wykrywania konfliktów akcji złożonych.

**Do uzgodnienia:**
- [ ] Reprezentacja `State` — `uint64` bitmapa (limit ~64 fluentów) vs `*big.Int` (bez limitu, wolniejsze) vs sortowany `string` (czytelne, ale wolne porównania)? Wpływa na cały silnik z O2.

**TODO (kolejno):**
1. [ ] Typy bazowe: `Fluent`, `Action`, `State` + helpery (równość, hash/klucz, `String()`).
2. [ ] AST formuł (`Formula` + warianty: literał, koniunkcja, alternatywa, negacja, ⊤/⊥).
3. [ ] `Eval(formula, state) bool` — ewaluacja na stanie.
4. [ ] `ToDNF(formula) []Conjunct` — konwersja do DNF.
5. [ ] AST `Statement` (6 wariantów) + marker interface.
6. [ ] `Domain` jako kontener + helpery: `Fluents()`, `Actions()`, `InertialFluents()`, `Always()`, `Causes(a)`, `Releases(a)`, ...
7. [ ] Dyrektywy `//go-sumtype:decl` na obu interface'ach.
8. [ ] Unit testy: ewaluacja formuł, DNF na kilku przykładach.

---

### O2. Dynamika akcji prostej (action language: semantyka)

**TL;DR:** Dla zadanej dziedziny zbuduj zbiory stanów dopuszczalnych Σ i początkowych Σ₀ oraz funkcję przejścia `Res(action, state)` — wszystko *tylko dla akcji prostych*.

**Estimate:** L.

**Wejście:** `Domain` z O1.
**Wyjście:** struktura z polami `Σ`, `Σ₀` (zbiory stanów) + metoda `Res(Action, State) map[State]struct{}`.

**Lokalizacja:** `internal/core/engine/` (np. `dynamics.go`).

**Dependencje:** używa typów + ewaluacji formuł z O1. Eksportuje `Res`/`Σ`/`Σ₀` do O3.

**Założenia:**
- Operuje **wyłącznie na akcjach prostych** — akcje złożone i procesy żyją w O3.
- Σ generowane z `always` (kandydaci ⊆ 2^|F| przefiltrowani przez koniunkcję `always`).
- Σ₀ generowane z Σ filtrowanego przez `initially` + `after` + `observable after` — `after`/`observable after` wymagają **forward** stosowania `Res` na akcjach prostych z każdego σ ∈ Σ (sprawdzamy, czy w jakimś/każdym osiągniętym stanie zachodzi α). To samo `Res` co w „normalnej" ewaluacji — żadnej regresji / pre-image. Dlatego Σ₀ należy do O2, nie do O5 (PK2 §4.2, §8.3).
- `Res` zgodne z PK2 §4: `Res₀` z `causes`/`releases`, **minimalizacja `New` na fluentach inercyjnych $\mathcal{F}_I$** (z O1), niedeterminizm jako wiele stanów wyjściowych. Niewykonalność akcji wynika z desugaringu `impossible A if π ≡ A causes ⊥ if π` (O4): gdy w `Res₀` pojawia się ⊥, żaden stan nie spełnia warunku → `Res` zwraca pusty zbiór. Bez osobnej reguły „priorytet impossible".
- Ramifikacje (Z6) z `always` realizujemy *przez ograniczenie kandydatów `Res₀` do Σ* — żadnego osobnego mechanizmu „indirect effects".
- `State` z O1 jest porównywalny → `map[State]struct{}` działa jako set bez triku.

**Do uzgodnienia:**
- [ ] Eager vs lazy generacja Σ. Eager (pełna enumeracja) jest prostsze, ale 2^|F| eksploduje powyżej ~20 fluentów. Lazy (generuj na żądanie z cache) komplikuje O2/O3. Zaczynamy od eager, dodajemy hard limit i komunikat błędu, czy od razu lazy?

**TODO (kolejno):**
1. [ ] Generacja Σ z `always`.
2. [ ] `Res₀(a, s)` — kandydaci po `causes`/`releases` (bez minimalizacji).
3. [ ] `New(s, s')` — różnica fluentów ograniczona do $\mathcal{F}_I$.
4. [ ] `Res(a, s)` — minimalizacja `Res₀` względem `New`, filtracja przez Σ (Z6); pusty wynik gdy `Res₀` zawiera ⊥ (po desugarowanym `impossible`).
5. [ ] Filtracja Σ → Σ₀ przez `initially`.
6. [ ] Filtracja Σ₀ przez `after π_1, ..., π_n` (forward `Res`).
7. [ ] Filtracja Σ₀ przez `observable after π_1, ..., π_n`.
8. [ ] Unit testy na małych dziedzinach (1–2 fluenty, 1–2 akcje, ręcznie wyliczone Σ/Σ₀/Res).

---

### O3. Query language — akcje złożone, procesy, kwerendy

**TL;DR:** Ewaluuj kwerendy Q1/Q2 × {necessary, possibly} na sparsowanej dziedzinie; zwróć `bool` + drzewo trace'a.

**Estimate:** XL — to najmięsistszy obszar.

**Wejście:** `Domain` (O1), `Σ`/`Σ₀`/`Res` (O2), `Query` AST (O4).
**Wyjście:** `bool` + `*TraceNode`. Dla Q2 dodatkowo zbiór stanów liściowych spełniających γ.

**Lokalizacja:** `internal/core/engine/` (np. `compound.go`, `process.go`, `query.go`, `trace.go`).

**Dependencje:** typy z O1, `Res`/`Σ`/`Σ₀` z O2, DNF z O1 (do AC). Konsumowane przez O5.

**Założenia:**
- **Nie buduje Σ ani Σ₀** — bierze gotowe z O2.
- Akcje złożone (PK2 §5): mechanizm AC — graf konfliktu między akcjami prostymi (warunki ich `causes`/`releases` w DNF się wykluczają), dekompozycje na maksymalne niezależne podzbiory, `Res(A, σ) = ⋃ Res(d, σ)` po dekompozycjach `d`. *Słabszy warunek „rozłącznych fluentów" z treści projektu nie wystarczy — implementujemy AC.*
- Ewaluacja procesu `Ψ = A_1; ...; A_n` jest **drzewem**, nie zbiorem stanów (PK2 §8.7) — inaczej `necessary executable` policzy się błędnie.
- `TraceNode` ma `Children []*TraceNode` (gałęzie dla niedeterminizmu `Res`), pole „powód niewykonalności" (np. `impossible`, brak akcji w `Res`), referencję do stanu i akcji. Drzewo zakorzenione w każdym σ₀ ∈ Σ₀.
- Kwantyfikatory (PK2 §7.3): `necessary = ∀σ₀ ∀Ψ`; `possibly = ∃σ₀ ∃Ψ`. Q1 — wykonalność procesu; Q2 — wykonalność + cel γ. `necessary γ after P` ⇒ `necessary executable P`.
- Bez krótkich circuit-breakerów dla `necessary` — zawsze liczymy pełne drzewo (nawet jak pierwsza gałąź zwróci `false`), żeby UI w O7 dostał kompletny trace do debugu.

**Do uzgodnienia:**
- [ ] Czy ewaluacja procesu ma hard-limit głębokości (np. 1000 kroków) z błędem czy biegnie bez ograniczenia? Z procesami iteracyjnymi można wpaść w nieskończoność.

**TODO (kolejno):**
1. [ ] Definicja `TraceNode` w `internal/core/engine/trace.go` zgodna z `api.Branch`/`api.Step` z `internal/api/result.go` (drzewo: `Children []*TraceNode`, `BlockedReason`, snapshot stanu i akcji).
2. [ ] Detekcja konfliktu między akcjami prostymi przez DNF warunków `causes`/`releases`.
3. [ ] Generacja maksymalnych niezależnych dekompozycji akcji złożonej (graf konfliktu → niezależne podzbiory).
4. [ ] `Res(A, σ)` dla akcji złożonej = suma `Res(d, σ)` po dekompozycjach.
5. [ ] Ewaluacja procesu `Ψ` jako drzewa — rekurencyjnie aplikuj `Res(A_i, σ)`, każda gałąź = jeden stan w wyniku.
6. [ ] Q1 × {necessary, possibly}: wykonalność (każda/jakaś gałąź się nie blokuje).
7. [ ] Q2 × {necessary, possibly}: dodatkowo sprawdzenie γ w liściach.
8. [ ] Unit testy na przykładach z O6 (oczekiwane TAK/NIE liczone ręcznie z PK2 §10).

---

### O4. Parser — text → AST (participle)

**TL;DR:** Sparsuj tekst dziedziny i kwerendy do typów z O1/O3 przy użyciu `alecthomas/participle/v2` (gramatyka w tagach pól).

**Estimate:** M.

**Wejście:** dwa stringi (`domainText`, `queryText`).
**Wyjście:** `Domain` (O1) lub `Query` (kształt z O3); w przypadku błędu `*Error{Kind: "syntax", Location: {Line, Column}, Message}`.

**Lokalizacja:** `internal/core/parser/` (`domain.go`, `query.go`, `lexer.go`, `normalize.go`).

**Dependencje:** używa `alecthomas/participle/v2`. Produkuje typy O1 (`Domain`, formuły) i typ kwerendy z O3.

**Założenia:**
- Gramatyka żyje w tagach pól struktur — `@Ident`, `"causes"`, `@@`, `(... | ...)`, `?`, `*`, `+`. Definicja typu = definicja składni.
- 8 zdań akcji z PK2 §3.2: `causes`, `releases`, `impossible`, `always`, `noninertial`, `initially`, `after`, `observable after`.
- Kwerenda: akcje złożone, procesy, Q1/Q2 × {necessary, possibly}.
- **Desugaring** w `normalize.go` przed wydaniem `Domain` na zewnątrz pakietu:
  - `impossible A if π` ≡ `A causes ⊥ if π`,
  - `initially α` ≡ `α after ε` (pusty proces),
  - `noninertial f` *zostaje* w AST — to **nie** cukier.
- Lokalizacja błędów (linia, kolumna) z `lexer.Position` participle. Format zgodny z `Error{Kind, Location, Message}` z O5.

**Do uzgodnienia:**
- [ ] Lexer — wbudowany `lexer.Default` czy custom rules? Słowa kluczowe (`causes`, `releases`, `if`, `always`, …) case-sensitive czy insensitive? `Fluent` vs `Action` — wspólny `Ident` discriminowany syntaktycznie czy oddzielne klasy tokenów?

**TODO (kolejno):**
1. [ ] Struktury participle dla 8 zdań akcji + formuły (raw AST), zgodnie z decyzją lexerową z „Do uzgodnienia".
2. [ ] `participle.Build(...)` + `Parse(text)` → raw AST.
3. [ ] `normalize(rawDomain) Domain` — desugar `impossible`/`initially`, walidacje sklejające z O1.
4. [ ] Struktury participle dla kwerendy (akcje złożone, procesy, Q1/Q2 × {nec, pos}).
5. [ ] Mapowanie błędów participle → `Error{Kind: "syntax", Location, Message}`.
6. [ ] Unit testy: golden-parse na każdym przykładzie z O6 + kilka błędnych inputów.

---

### O5. Facade i testy e2e

**TL;DR:** Zaimplementuj publiczny kontrakt `internal/api` (`Solve`, `ValidateDomain`, `ListExamples`, `LoadExample`) i e2e testy przez `goldie`.

**Estimate:** M.

**Wejście:** stringi (z frontendów).
**Wyjście:** `SolveResult` / `ValidationResult` / `[]ExampleSummary` / `Example` — wszystkie z tagami `json:"..."`.

**Lokalizacja:** `internal/api/` (`api.go`, `result.go`); testy w `tests/`.

**Dependencje:** orkiestruje O4 → O1 → O2 → O3; konsumuje przykłady z O6. Jedyny kontrakt dla O7/O7b.

**Założenia:**
- **Zero typów core'a w API.** `Domain`, `State`, `TraceNode` z core'a *nie wyciekają* — facade pakuje wszystko w struct types z `internal/api/result.go`.
- **`context.Context` jako pierwszy parametr każdej funkcji facade'u** (`Solve`, `ValidateDomain`, `ListExamples`, `LoadExample`). Cancel propaguje się przez `engine.Evaluate`/`engine.Sigma`/`engine.Sigma0` (które też biorą ctx) i kończy `Solve` z `Answer: AnswerError`, `Error{Kind: "internal", Message: "cancelled"}`. O7 trzyma `ctx, cancel` per-Solve i wpina cancel w przycisk; O7b używa wprost `r.Context()` (zamknięcie zakładki = anulowanie). Wewnątrz silnika ctx jest sprawdzany przy granicach σ₀ i dekompozycji procesu — *nie* w hot pathie `Res`.
- `SolveResult` musi unieść: (a) odpowiedź TAK/NIE, (b) trace z O3 (przepakowany), (c) dla Q2 — stany liściowe spełniające γ, (d) podsumowanie `|Σ|`, `|Σ₀|`, czas.
- Walidacja semantyczna na poziomie modelu: pusta Σ (sprzeczne `always`) lub pusta Σ₀ (sprzeczne `initially`/`after`/`observable after`) → `Error{Kind: "semantic"}`. `Solve()` zatrzymuje się przed ewaluacją kwerendy.
- Tagi `json:"..."` na wszystkich polach — web wariant marshaluje przez `encoding/json`, Fyne ignoruje tagi bez kosztu.
- E2e testy idą **wyłącznie przez facade** — nie wołają `internal/core/...` bezpośrednio. Każdy przykład × każda kwerenda z O6 = jeden golden plik (`goldie.AssertJson`).
- `Summary.ElapsedMs` mierzony wall-clockiem (`time.Now()`) — wystarczy dla paska statusu w UI.
- Facade jest cichy w PK3 (zero `slog`/`log`); wszystko, co istotne, leci jako pole `Error` w odpowiedzi. Logi można dodać w PK4 bez zmiany kontraktu.

**TODO (kolejno):**
1. [ ] Definicja typów wynikowych w `result.go`: `SolveResult`, `Step`, `Branch`, `Error`, `ValidationResult`, `ExampleSummary`, `Example`.
2. [ ] `Solve(ctx context.Context, domain, query string) SolveResult` — orkiestracja: parse → validate → eval → pack. Honoruj `ctx.Err()` po każdej fazie.
3. [ ] `ValidateDomain(ctx context.Context, domain string) ValidationResult` — parse + sprawdzenie pustego modelu.
4. [ ] `ListExamples(ctx) []ExampleSummary` i `LoadExample(ctx, id) (Example, *Error)` — opakowują `examples.Load()` + `examples.ReadDomain()` z O6.
5. [ ] Pierwszy e2e (smoke) na jednym przykładzie z O6, zapis goldena.
6. [ ] Pełen sweep e2e: każdy przykład × każda kwerenda z O6.

---

### O6. Oracle przykładów (ground truth)

**TL;DR:** Dostarcz wbudowane przykłady (dziedzina + kwerendy + oczekiwane odpowiedzi) — jednocześnie zawartość menu *Przykłady* w GUI i oracle dla testów e2e.

**Estimate:** M (większość pracy = ręczne wyprowadzanie odpowiedzi z semantyki, nie kod).

**Wejście:** PK2 §10 (cztery wyprowadzone przykłady).
**Wyjście:** pliki w `internal/examples/` wbudowane przez `//go:embed`.

**Lokalizacja:** `internal/examples/`.

**Dependencje:** konsumowane przez O5 (`ListExamples`/`LoadExample` + oracle e2e) i pośrednio przez O7/O7b (menu *Przykłady*).

**Założenia:**
- Każdy przykład = plik z dziedziną + 2–3 kwerendy + dla każdej kwerendy: oczekiwana odpowiedź TAK/NIE i krótka notatka „skąd" (jakie σ₀, jak idzie trace).
- Łącznie przykłady mają pokrywać Z1, Z3, Z5, Z6, **Z7** (inercja, niedeterminizm, niewykonalność, ramifications, `|Σ₀| > 1`) oraz wszystkie cztery formy kwerend (Q1/Q2 × {necessary, possibly}).
- **Domyślny zestaw**: cztery przykłady z PK2 §10 — YSP z rosyjską ruletką, producent i konsumenci, dwa przełączniki, dwóch robotników.
- Format pliku skomitowany w `internal/examples/manifest.json`: każdy przykład = `<id>.txt` z dziedziną + wpis w `manifest.json` z `{id, title, file, tags[], queries: [{text, expected, note}]}`. Kwerendy są inline w manifescie (nie ma osobnego `.queries.json`).

**Do uzgodnienia:**
- [ ] Czy dorzucamy „negatywne" przykłady (sprzeczna dziedzina → pusta Σ; sprzeczna inicjalizacja → pusta Σ₀) jako oracle dla walidacji semantycznej z O5?

**TODO (kolejno):**
1. [ ] Dla każdego z 4 przykładów: ręczne wyprowadzenie Σ, Σ₀, drzew procesu, odpowiedzi na 2–3 kwerendy.
2. [ ] Przepisanie dziedzin do plików `.txt` w składni DS4 (zgodnej z O4 — dogadać po starcie O4).
3. [ ] Wpisanie kwerend + odpowiedzi do `manifest.json` (`{id, title, file, tags[], queries: [{text, expected, note}]}` per przykład).
4. [ ] Dyrektywa `//go:embed manifest.json data/*.txt` w pakiecie `internal/examples/` (już jest — utrzymać).
5. [ ] API pakietu: `Load() (*Manifest, error)` + `ReadDomain(Entry) (string, error)` dla O5 (już istnieje — uzupełnić jeśli nowe potrzeby wyjdą podczas O5).

---

### O7. GUI Fyne (primary)

**TL;DR:** Zbuduj okno desktop z trzema strefami (dziedzina / kwerenda+wynik / status) opakowujące `internal/api`.

**Estimate:** M.

**Wejście:** `internal/api`.
**Wyjście:** binary `cmd/ds4/ds4.exe`.

**Lokalizacja:** `cmd/ds4/` (`main.go` + ewentualne pliki widget'ów obok).

**Dependencje:** wyłącznie `internal/api` (`depguard` pilnuje, że zero importów `internal/core/...` ani `fyne.io/...` w core'rze).

**Założenia:**
- Layout: `container.NewHSplit` (lewa: dziedzina | prawa: `container.NewVSplit` (kwerenda+button | wynik)) + pasek statusu na dole.
- Lewa: gołe `widget.Entry` (multilinia, monospace) na dziedzinę. Bez syntax highlightingu — Fyne nie ma tego out-of-the-box, custom widget = za duża praca na PK3.
- Prawa góra: `widget.Entry` na kwerendę + `widget.Button("Oblicz")` → `api.Solve(ctx, ...)` w gorutynie. Drugi przycisk „Anuluj" wywołuje `cancel()` z `ctx, cancel := context.WithCancel(parent)`. Walidacja dziedziny dzieje się tylko przy `Oblicz` (bez live-debounce).
- Prawa dół: `widget.Label` (TAK/NIE/błąd) + `widget.Tree` (trace z `SolveResult`) + dla Q2 lista stanów spełniających γ.
- Pasek statusu: `|Σ|`, `|Σ₀|`, czas, ostatni błąd walidacji.
- Menu *Przykłady* (`fyne.Menu`) zasilane z `api.ListExamples(ctx)`; klik → `api.LoadExample(ctx, id)` → wypełnij oba edytory; przy 2–3 kwerendach per przykład — dropdown nad polem kwerendy (web wariant zrobi tak samo dla spójności UX).
- Renderer błędów wyświetla `Error.Location` (linia/kolumna) z O4.
- **Zero logiki decyzyjnej** w GUI — „akcja niewykonalna w σ₃ bo X" pochodzi z O3 i jest tylko wyświetlana.
- Dev na macOS: `go run ./cmd/ds4`. Final build natywnie na Windowsie.

**TODO (kolejno):**
1. [ ] Szkielet `main.go` — `app.New()`, `window.SetContent(...)`, `widget.Entry × 2`, button.
2. [ ] Layout splittera + pasek statusu.
3. [ ] Renderer `SolveResult` (Label + Tree dla trace).
4. [ ] Renderer błędów z linią/kolumną (z `Error.Location`).
5. [ ] Dla Q2: lista stanów spełniających γ.
6. [ ] Menu *Przykłady* przez `api.ListExamples(ctx)`/`api.LoadExample(ctx, id)`.
7. [ ] Smoke run na każdym przykładzie z O6 (wzrokowo).

---

### O7b. Web frontend (fallback, conditional)

**TL;DR:** Plan B — HTTP server + SPA opakowujące to samo `internal/api`. Aktywujemy decyzją zespołu, jeśli Fyne padnie.

**Estimate:** M (warunkowo — domyślnie nie kodujemy).

**Wejście:** `internal/api`.
**Wyjście:** binary `cmd/ds4-web/ds4-web.exe` (single binary z embedowanym SPA).

**Lokalizacja:** `cmd/ds4-web/main.go` + `cmd/ds4-web/static/{index.html, app.js, style.css}` (przez `//go:embed`).

**Dependencje:** wyłącznie `internal/api` (depguard); konsumuje przykłady przez API, nie czyta `internal/examples` bezpośrednio.

**Założenia:**
- Ten sam `internal/api` co Fyne — zero duplikacji logiki.
- SPA: HTML + CSS + vanilla JS, **żadnego frameworka, żadnego npm**.
- Stack runtime'owy: stdlib `net/http` + `encoding/json`. Zero nowych zależności.
- Endpointy (każdy handler przekazuje `r.Context()` do `api.*` — zamknięcie zakładki = anulowanie in-flight `Solve`):
  - `GET /` → `index.html`.
  - `GET /api/examples` → lista (z `api.ListExamples(ctx)`).
  - `GET /api/examples/{id}` → treść (z `api.LoadExample(ctx, id)`).
  - `POST /api/solve` body `{domain, query}` → `SolveResult` JSON (`api.Solve(ctx, domain, query)`).
  - `POST /api/validate` body `{domain}` → `ValidationResult` JSON (`api.ValidateDomain(ctx, domain)`).
- Trace tree w SPA: zagnieżdżone `<details>`/`<ul>`, zero zależności frontendowych.
- Autostart przeglądarki: pierwszy wolny port z 8080–8090, OS-specific komenda (`open` na macOS, `cmd /c start ""` na Windowsie, `xdg-open` na Linuksie).
- Zamknięcie zakładki *nie* zamyka serwera (robi to konsola / Ctrl+C).

**Trigger aktywacji (decyzja zespołu):**
- Fyne nie kompiluje się stabilnie na Windowsie, **lub**
- Fyne renderuje nieczytelnie na realnym przykładzie, **lub**
- Smoke test pokazuje crash/freeze.

Domyślnie: **no** — `cmd/ds4-web/` zostaje w repo jako preview do PK4.

**TODO (warunkowo, po decyzji go):**
1. [ ] Pliki SPA w `cmd/ds4-web/static/` (`index.html`, `app.js`, `style.css`).
2. [ ] HTTP handlery w `main.go` opakowujące `api.Solve`/`Validate`/`ListExamples`/`LoadExample`.
3. [ ] `//go:embed static/*` + `http.FileServer` na `embed.FS`.
4. [ ] Trace tree w SPA jako `<details>`/`<ul>`.
5. [ ] Cross-platform browser autostart (`runtime.GOOS` switch).
6. [ ] Wybór portu: pętla 8080–8090 do pierwszego wolnego.
7. [ ] Krótkie README dla użytkownika końcowego: „uruchom `ds4-web.exe`, otworzy się przeglądarka, nie zamykaj okna konsoli".

---

### O8. Pakowanie + Windows-build

**TL;DR:** Zbuduj single-binary `.exe` na Windows i wykonaj smoke-test na czystej maszynie. Wstaw lint + depguard w CI.

**Estimate:** S.

**Wejście:** drzewo źródeł.
**Wyjście:** artefakt `ds4.exe` (i opcjonalnie `ds4-web.exe`) + zielony `golangci-lint`.

**Lokalizacja:** `Makefile` (lub `taskfile.yml`) + `.golangci.yml` w korzeniu.

**Dependencje:** pakuje O7 (Fyne) i opcjonalnie O7b. Wymaga gotowego `internal/api` do smoke-testu.

**Założenia:**
- **Primary (Fyne):** build natywnie na Windowsie — `go build -ldflags="-s -w" -o ds4.exe ./cmd/ds4`. Cgo + OpenGL + mingw cross-compile świadomie omijamy.
- **Fallback (web):** cross-compile z dowolnej platformy — `GOOS=windows GOARCH=amd64 go build -ldflags="-s -w" -o ds4-web.exe ./cmd/ds4-web`. Bez cgo, działa z Maca bez Dockera.
- Single binary statyczny w obu wariantach, ~15 MB, brak runtime'u u użytkownika.
- `golangci-lint` z włączonym:
  - `depguard` — `internal/core/...` nie importuje `fyne.io/...` ani `net/http`.
- **`go-sumtype` jako osobne narzędzie** (`make sumtype`, nie golangci-lint) — exhaustiveness na `Statement`/`Formula`/`Query`. `go-sumtype` *nie jest* wbudowanym linterem `golangci-lint`, więc trzymamy go jako osobny cel w Makefile (`go install github.com/BurntSushi/go-sumtype@latest` → `go-sumtype ./...`). CI uruchamia oba: `make lint` + `make sumtype`.
- **Smoke-test artefaktu na fizycznym czystym Windowsie obowiązkowy** przed PK3 (laptop kolegi z zespołu / lab PW).

**Do uzgodnienia:**
- [ ] CI — GitHub Actions z Windows runnerem dla build-fyne (mamy budżet darmowych minut?), czy build robi człowiek lokalnie i wrzuca artefakt do repo?
- [ ] Antivirus / Defender SmartScreen false-positives. Statyczne Go-binaries bywają fałszywie wykrywane. Podpisać `.exe` certem (kto kupuje?) czy oddać niepodpisany z notką w README, że to znany efekt?

**TODO (kolejno):**
1. [ ] Cele Makefile: `dev`, `test`, `lint`, `build-fyne` (na Win), `build-web` (cross).
2. [ ] `.golangci.yml` z `depguard` (architecture rule). Osobno: cel `make sumtype` w Makefile uruchamiający `BurntSushi/go-sumtype` na całym repo (exhaustiveness na `Statement`/`Formula`/`Query`).
3. [ ] Pierwszy `hello.exe` ze szkieletem z O7 — smoke-test toolchainu na czystym Windowsie *przed* zaczęciem porządnego kodu.
4. [ ] Po skończeniu O7 — finalny build + smoke-test na czystym Windowsie z każdym przykładem z O6.
