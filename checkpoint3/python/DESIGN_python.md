# Checkpoint 3 — Design (v2, high-level)

> Zakres tego dokumentu: cele PK3, decyzje techniczne, architektura i UX — na poziomie ogólnym.
> Specyfikację dziedziny, składnię języków i semantykę bierzemy z:
> - [`docs/Projekt_nr_4___2026.pdf`](../docs/Projekt_nr_4___2026.pdf) — treść projektu (Z1–Z7, Q1/Q2),
> - [`checkpoint2/checkpoint2.pdf`](../checkpoint2/checkpoint2.pdf) — sformalizowana teoria (do poprawienia wg uwag z PK2 przed PK4).

## 1. Cele i decyzje techniczne

### 1.1. Cele PK3 (12.05)

- **C1.** Aplikacja desktop GUI: edytor dziedziny + edytor kwerendy → odpowiedź TAK/NIE + krok-po-kroku trace procesu.
- **C2.** Pojedynczy `.exe` na Windows (PyInstaller), uruchamialny dwuklikiem na czystej maszynie.
- **C3.** Wbudowane przykłady z menu (kilka, dobrane tak, by łącznie pokryć Z1, Z3, Z5, Z6 i oba kwantyfikatory), każdy z 2–3 kwerendami i znanymi oczekiwanymi odpowiedziami; oczekiwane odpowiedzi pełnią rolę oracle'a w testach. Konkretny zestaw — do uzgodnienia w O6.
- **C4.** Core (parser + engine) jako pure-Python z zerową zależnością od UI — żeby alternatywny frontend (np. web) był *możliwy*, nawet jeśli go nie budujemy.
- **C5.** Testy: pokrycie każdego modułu core'a + e2e na każdy przykład przez facade.

### 1.2. Decyzje techniczne

- **Python 3.9+** — najszerszy realny zakres; dataclasses i generyki natywne (PEP 585: `list[…]`, `dict[…]`) są dostępne. Świadomie nie używamy `match` (3.10+) ani `X | Y` w runtime'owych aliasach (3.10+) — w nich używamy `typing.Union`.
- **Tkinter (stdlib)** jako jedyna technologia UI — żeby nie dorzucać zależności runtime do `.exe` (C2).
- **Pure-Python core, bez importów UI** — wymóg z C4; pozwala testować engine bez GUI i potencjalnie podmienić frontend.
- **Ręczny parser recursive descent**, bez ANTLR/Lark — składnia DS4 jest mała i zna ją cały zespół; zewnętrzna gramatyka byłaby narzutem na build i naukę.
- **PyInstaller** do produkcji `.exe` — to najprostsze narzędzie spełniające C2.
- **pytest** — standardowy runner, jednolity format dla unit-testów i golden-testów przykładów.
- **Granica frontend ↔ core: `str` na wejściu, JSON-able `dict` na wyjściu** — dzięki temu cały kontrakt da się serializować, mockować w testach i podpiąć do innego frontendu bez zmian w core.

## 2. Architektura

```
┌─────────────────────────────────────────────────────────────┐
│ FRONTEND  (Tkinter)  —  zna tylko facade                     │
└──────────────────────────┬──────────────────────────────────┘
                           │  str → solve() → dict
┌──────────────────────────▼──────────────────────────────────┐
│ FACADE  (ds4.api)                                            │
│   solve · validate_domain · list_examples · load_example     │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ CORE  (pure-Python)                                          │
│                                                               │
│   ACTION LANGUAGE          │   QUERY LANGUAGE                 │
│   model, formuły, Σ/Σ₀,    │   akcje złożone, procesy,        │
│   res₀ / new / res         │   Q1/Q2 × {necessary, possibly}  │
│                                                               │
│   PARSER                                                      │
│   lexer · domain_parser · query_parser                        │
└─────────────────────────────────────────────────────────────┘
```

**Zasady architektoniczne:**

1. **Core nie wie o UI.** Zero importów `tkinter` w `ds4/`; facade zwraca tylko JSON-serializowalne dict-y. To trzyma core testowalnym i otwartym na alternatywny frontend bez pisania go.
2. **Action language ≠ query language.** Akcje złożone i procesy żyją wyłącznie po stronie query language — osobny moduł, osobny parser, osobne testy.
3. **Składnia i semantyka biorą się z PK2.** Język akcji ma 8 rodzajów zdań (PK2 §3.2): `causes`, `releases`, `impossible`, `always`, `noninertial`, `initially`, `after`, `observable after`. Cukier syntaktyczny: `impossible A if π` ≡ `A causes ⊥ if π`, `initially α` ≡ `α after ε` — desugaring po stronie parsera (O4). **`noninertial f` *nie* jest cukrem** — to osobne zdanie strukturalne wyznaczające zbiór fluentów inercyjnych $\mathcal{F}_I$ używany przez `New` w O2 (PK2 §4.4); musi przeżyć w AST i być eksponowane przez `Domain`. Pozostałe zdania (`causes`, `releases`, `always`, `noninertial`, `after`, `observable after`) trzymamy w AST. Konkretny kształt węzłów — decyzja O1+O4.
4. **Facade jest jedynym kontraktem dla frontendu.**

## 3. UX

```
┌──────────────────────────────────────────────────────────────────────┐
│ DS4 Reasoner                                              _ □ ✕      │
├──────────────────────────────────────────────────────────────────────┤
│ [Plik] [Przykłady] [Pomoc]                                            │
├──────────────────────────────────┬──────────────────────────────────┤
│  DZIEDZINA  (edytor)              │  KWERENDA  (edytor)  [▶ Oblicz]  │
│                                   ├──────────────────────────────────┤
│                                   │  WYNIK  (odpowiedź + trace)      │
├──────────────────────────────────┴──────────────────────────────────┤
│ ● Status · |Σ|, |Σ₀| · czas obliczenia · błędy walidacji              │
└──────────────────────────────────────────────────────────────────────┘
```

**Edytor dziedziny (lewa).** Pole tekstowe na opis dziedziny.

**Edytor kwerendy + Oblicz (prawa góra).** Pole na pojedynczą kwerendę i przycisk uruchamiający `solve()`.

**Panel wyniku (prawa dół).** Odpowiedź TAK/NIE/błąd, liczbowe podsumowanie (`|Σ|`, `|Σ₀|`), trace procesu krok po kroku z oznaczeniem stanów, w których akcja złożona jest niewykonalna i krótkim uzasadnieniem; dla Q2 dodatkowo stany końcowe spełniające `γ`.

**Menu *Przykłady*.** Wbudowane przykłady — jedno kliknięcie wypełnia oba edytory. Te same wejścia są oracle'em w testach e2e (C3, C5).

## 4. Proponowany podział obszarów odpowiedzialności

### O1. Model i formuły (action language: dane)

**Co robi:** definiuje wszystkie typy danych domeny i logikę formuł zdaniowych. Wejście: nic (fundament). Wyjście: importowalne klasy + funkcje na formułach.

**Założenia:**
- Niezależny od parserów i od dynamiki — to czysta warstwa danych.
- AST pokrywa zdania PK2 §3.2 po desugaringu (`impossible`, `initially` znikają, reszta zostaje).
- `Domain` eksponuje pochodne potrzebne dalej, m.in. zbiór fluentów inercyjnych $\mathcal{F}_I$ (z `noninertial`) — używany przez O2 w `New`.
- Formuły: AST + ewaluacja na stanie + **DNF** (PK2 §5.2 i §8.0) — wymagany przez O3 do wykrywania konfliktów.

**TODO:**
- `Fluent`, `Action`, `State`, `Domain`, `Statement` (warianty wg decyzji O1, zgodnie z punktem 3 z §2).
- AST formuł, ewaluacja na stanie, konwersja do DNF.

**Łączy się z:** O2 (eksportuje typy), O3 (eksportuje typy + ewaluację), O4 (parser produkuje `Domain`), O5 (facade konsumuje `Domain` i `State`).

### O2. Dynamika akcji prostej (action language: semantyka)

**Co robi:** dostarcza całą „fizykę" dziedziny dla pojedynczej akcji oraz buduje model na poziomie akcji prostych. Wejście: `Domain` z O1. Wyjście: `Σ`, `Σ₀` oraz `res(action, state) → set[State]`.

**Założenia:**
- Operuje wyłącznie na akcjach prostych — akcje złożone i procesy są poza tym obszarem (O3).
- Σ, Σ₀ i `res` zgodne z definicjami z PK2 §4 (łącznie z minimalizacją `New` przy fluentach inercyjnych $\mathcal{F}_I$ z O1, niedeterminizmem i priorytetem `impossible`); reszta systemu widzi tylko `set[State]`.
- **Σ₀ należy do O2.** Filtracja Σ₀ przez `initially`, `after` i `observable after` (PK2 §4.2, §8.3) wymaga odwrotnego stosowania `res` dla akcji prostych — czyli rzeczy, które już są w O2. O3 tej filtracji nie dotyka.
- Ramifikacje (Z6) z `always` są realizowane wyłącznie przez ograniczenie kandydatów w `res₀` do Σ — nie ma osobnego mechanizmu „indirect effects".

**TODO:**
- Generacja Σ z `always`; generacja Σ₀ z `initially` + `after` + `observable after`.
- `res₀`, `new`, `res` dla akcji prostej.

**Łączy się z:** O1 (typy + ewaluacja formuł), O3 (eksportuje `res`, `Σ`, `Σ₀`), O5 (facade konsumuje `Σ`, `Σ₀` w wyniku).

### O3. Query language — akcje złożone, procesy, kwerendy

**Co robi:** odpowiada na kwerendy. Wejście: `Domain`, `Σ₀` i sparsowana kwerenda. Wyjście: bool + trace strukturyzowany dla GUI.

**Założenia:**
- Korzysta z `res` z O2, typów z O1 oraz **DNF z O1** (do wykrywania konfliktów wg PK2 §5.2). Sam nie buduje Σ ani Σ₀.
- Akcje złożone, dekompozycje i `res(A,σ)` zgodnie z PK2 §5 (mechanizm AC: graf konfliktu + dekompozycje). Uwaga: warunek „rozłącznych fluentów" z treści projektu jest *słabszy* — nie wystarczy do PK4. Implementujemy AC.
- Semantyka kwantyfikatorów wg PK2 §7.3: `necessary` = ∀σ₀ ∀Ψ; `possibly` = ∃σ₀ ∃Ψ. Q1 patrzy na wykonalność procesu, Q2 dodatkowo na cel `γ`; `necessary γ after P` implikuje `necessary executable P`.
- Ewaluacja procesu jest **drzewem** (PK2 §8.7), nie zbiorem stanów osiągalnych — inaczej `necessary executable` policzy się błędnie. Trace ma kształt drzewa zakorzenionego w każdym σ₀ ∈ Σ₀, z gałęziami dla niedeterminizmu `Res` i z polem „powód niewykonalności" w węzłach blokowanych. Konkretny kształt — decyzja O3 z O5/O7.

**TODO:**
- Akcje złożone wg PK2 §5 (konflikty, dekompozycje, `res` przez sumę po dekompozycjach).
- Procesy: iteracja `Ψ`, drzewo wykonania, agregacja stanów + trace.
- Q1, Q2 × {necessary, possibly} jako cztery funkcje ewaluujące drzewo + `γ`.

**Łączy się z:** O1 (typy), O2 (`res`, `Σ`, `Σ₀`), O4 (dostaje sparsowaną kwerendę), O5 (zwraca trace + odpowiedź).

### O4. Parser — text → AST

**Co robi:** tłumaczy tekst dziedziny i tekst kwerendy na typy z O1/O3. Wejście: stringi. Wyjście: `Domain`, `Query`, lub strukturalny błąd.

**Założenia:**
- Dwa osobne parsery: `domain_parser` (8 typów zdań z PK2 §3.2) i `query_parser` (akcje złożone, procesy, Q1/Q2 × {necessary, possibly}).
- Desugaring `impossible` i `initially` (patrz §2 zasada 3) — w jednym miejscu w parserze.
- Produkuje **typowane** AST: identyfikatory są windowane do `Fluent`/`Action` z O1 na podstawie pozycji składniowej (akcje pojawiają się tylko w pozycji akcji, fluenty tylko w formułach — brak konfliktu nazw). O5 nie powtarza tej pracy. Błędy: `kind="syntax"` + `Location` (linia, kolumna).

**TODO:**
- Lexer z lokalizacją tokenów.
- `domain_parser` produkujący `Domain` z O1.
- `query_parser` produkujący `Query` zgodny z oczekiwaniem O3.

**Łączy się z:** O1 (produkuje typy), O3 (produkuje `Query`), O5 (wywoływany z `solve` / `validate_domain`), O7 (przez format błędu).

### O5. Facade i testy e2e

**Co robi:** definiuje publiczny kontrakt API i jest jedynym punktem styku frontendu z resztą. Wejście: stringi. Wyjście: JSON-able dict (`SolveResult`, `ValidationResult` itp.).

**Założenia:**
- Facade orkiestruje O4 → O1/O2 → O3 i pakuje wynik w JSON-serializowalny `dict` (nie wycieka typami z core'a).
- Testy e2e idą wyłącznie przez facade. Każdy przykład × każda kwerenda z O6 = jeden test.
- Kształt zwracanych dict-ów żyje w `ds4/api.py` (`SolveResult`, `Step`, `Branch`, `Error`, …). `SolveResult` musi unieść (a) odpowiedź TAK/NIE, (b) trace z O3, (c) dla Q2 — stany końcowe spełniające `γ`, (d) podsumowanie ($|\Sigma|$, $|\Sigma_0|$, czas) dla paska statusu w O7.
- Walidacja semantyczna na poziomie modelu (`kind="semantic"`): pusta Σ (sprzeczne `always`), pusta Σ₀ (sprzeczne `initially` / `after` / `observable after`). `solve()` zatrzymuje się przed ewaluacją jeśli model jest pusty.

**TODO:**
- Implementacja `solve`, `validate_domain`, `list_examples`, `load_example` (te dwa ostatnie czytają z O6).
- Testy e2e konsumujące pliki + odpowiedzi z O6 i jadące przez facade.

**Łączy się z:** O4 + O1/O2 + O3 (wywołuje), O6 (konsumuje przykłady i oczekiwane odpowiedzi), O7 (jedyny kontrakt dla GUI).

### O6. Oracle przykładów (ground truth)

**Co robi:** dostarcza wbudowane przykłady i ich poprawne odpowiedzi — jednocześnie zawartość menu *Przykłady* w GUI i oracle dla testów e2e (C3, C5).

**Założenia:**
- Wymaga starannego wyprowadzenia odpowiedzi z semantyki, nie kodowania.
- Każdy przykład = plik z dziedziną + 2–3 kwerendy + dla każdej kwerendy: oczekiwana odpowiedź TAK/NIE i krótka notatka „skąd" (jakie σ₀, którędy idzie trace).
- Łącznie przykłady mają pokrywać Z1, Z3, Z5, Z6, **Z7** (inercja, niedeterminizm, niewykonalność, ramifications, częściowy opis stanu początkowego — czyli `|Σ₀| > 1`) oraz wszystkie cztery formy kwerend (`Q1` i `Q2`, każda w wariancie `necessary` i `possibly`).
- **Domyślny zestaw**: cztery przykłady wyprowadzone już w PK2 §10 (YSP z rosyjską ruletką, producent i konsumenci, dwa przełączniki, dwóch robotników) — odpowiedzi i trace są tam policzone, więc oracle praktycznie istnieje. Zestaw można rozszerzyć, ale nie ma powodu wymyślać od zera.
- Format plików (struktura katalogu, nazewnictwo, sposób trzymania oczekiwanych odpowiedzi) ustalany wspólnie z O5.

**Łączy się z:** O5 (oracle dla e2e + źródło dla `list_examples`/`load_example`), O7 (zawartość menu *Przykłady*).

### O7. GUI

**Co robi:** desktopowa aplikacja Tkinter — jedyny frontend dostarczany w PK3. Wejście: facade z O5. Wyjście: użyteczny program.

**Założenia:**
- Zero importów z `ds4.engine` / `ds4.parser` — GUI zna tylko facade.
- Renderuje `SolveResult` i błędy wprost ze struktury dict-a; cała logika decyzyjna („akcja niewykonalna w σ₃ bo X") pochodzi z O3 i jest tylko wyświetlana.
- Menu *Przykłady* zasilane przez `facade.list_examples()` / `load_example()` (które pod spodem czytają O6).

**TODO:**
- Layout głównego okna z trzema strefami i paskiem statusu.
- Renderer `SolveResult` (odpowiedź, podsumowanie, trace per-krok, dla Q2 stany spełniające γ).
- Renderer błędów z lokalizacją (linia/kolumna) z O4.
- Podpięcie menu *Przykłady* do facade.

**Łączy się z:** O5 (jedyny kontrakt).

### O8. Pakowanie + Windows-build

**Co robi:** zamienia źródło Pythona w pojedynczy `.exe` uruchamialny dwuklikiem na czystym Windowsie (C2). Wejście: drzewo źródeł. Wyjście: artefakt dystrybucji + dowód, że działa.

**Założenia:**
- Pierwszy artefakt: „pusty exe z zaślepką GUI" działający na czystym Windowsie — weryfikacja toolchainu przed finałowym pakowaniem.

**TODO:**
- Konfiguracja PyInstallera (entry point, ikona, ścieżki danych jeśli będą).
- Pierwszy `hello.exe` ze szkieletem z O7 — smoke-test toolchainu.
- Finalny build + test na czystym Windowsie.

**Łączy się z:** O7 (pakuje jego output), O5 (pakuje pakiet `ds4`), reszta — pośrednio.

## 5. Otwarte kwestie / do uzgodnienia

Drobiazgi, które trzeba rozstrzygnąć, ale nie blokują startu prac. Każdy punkt wymaga 15-minutowej decyzji właścicieli wymienionych w nawiasie.

- **Kształt `trace` i `Query` AST** (O3 + O4 + O5 + O7). Trace musi być drzewem (patrz O3) i unieść świadków γ dla Q2; konkretne pola węzła i shape `Query` — domówić zanim O3 i O7 napiszą cokolwiek niezmiennikowego.
- **`SolveResult` — pola statusu** (O5 + O7). `|Σ|`, `|Σ₀|`, czas obliczenia, świadki γ — albo wchodzą do `SolveResult`, albo znikają z UX. Wybrać.
- **Zachowanie `solve()` przy pustym modelu** (O5 + O7). Kiedy Σ = ∅ lub Σ₀ = ∅: błąd `kind="semantic"` z komunikatem, czy odpowiedź vacuous-true/false dla każdej kwerendy? Pinujemy konwencję, GUI ją renderuje.
- **Format plików przykładów** (O6 + O5 + O8). `pyproject.toml` zakłada `*.txt` w `ds4/examples` + odpowiedzi po stronie Pythona — uzgodnić układ pliku (jeden plik na przykład vs katalog) i jak są w nim trzymane kwerendy + oczekiwane odpowiedzi.
- **GUI a wiele kwerend per przykład** (O7 + O6). Po `load_example` przykład ma 2–3 kwerendy; menu pokazuje listę wyboru, dropdown czy podmenu — drobny UX, ale O7 musi wiedzieć.

