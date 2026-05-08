# Checkpoint 3 — Design dokumentu i podział pracy

## 1. Filozofia architektoniczna

**Główna zasada:** rdzeń (silnik + parser) jest **czystą biblioteką Python bez żadnych zależności GUI**. UI jest cienką nakładką, którą można wymienić.

```
┌──────────────────────────────────────────────────────────┐
│        FRONTEND (wymienny)                                │
│   ┌────────────────┐         ┌─────────────────────┐    │
│   │  Tkinter app   │   lub   │  Flask/FastAPI      │    │
│   │  (PK3 primary) │         │  + HTML/JS (PK4+)   │    │
│   └────────┬───────┘         └──────────┬──────────┘    │
└────────────┼─────────────────────────────┼───────────────┘
             │                             │
             ▼                             ▼
┌──────────────────────────────────────────────────────────┐
│        FACADE / API (ds4.api)                             │
│   - solve(domain_text, query_text) -> dict (JSON-able)    │
│   - load_example(name) -> dict                            │
│   - validate(domain_text) -> dict                         │
│   Wszystko: stringi w wejściu, dict w wyjściu             │
└──────────────────────────────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────────────────┐
│        CORE (ds4.engine + ds4.parser, pure Python)        │
│   - bez importów tkinter / flask / fastapi                │
│   - testowalne unit-testami                               │
│   - reużywalne w CLI, web, GUI, notebooku Jupyter         │
└──────────────────────────────────────────────────────────┘
```

**Konsekwencja:** ten sam `solve()` z facade działa w Tkinterze (PK3) i w Flasku (PK4 web wariant). Frontendy dzielą tylko stringi i JSON, niczego więcej.

## 2. Struktura katalogów

```
checkpoint3/
├── pyproject.toml              ← deklaracja paczki ds4
├── requirements.txt            ← customtkinter (opcjonalnie), pyinstaller
├── main.py                     ← entry point: uruchamia GUI Tkinter
├── main_web.py                 ← (opcjonalny) entry point Flask
├── ds4/                        ← główna paczka
│   ├── __init__.py
│   ├── api.py                  ← FACADE (publiczny interfejs)
│   ├── engine/
│   │   ├── __init__.py
│   │   ├── model.py            ← struktury danych (Fluent, Action, State, Statement)
│   │   ├── formula.py          ← AST formuł, ewaluacja, DNF
│   │   ├── states.py           ← generacja Σ, Σ₀
│   │   ├── transitions.py      ← Res₀, New, Res dla akcji prostych
│   │   ├── compound.py         ← Res dla akcji złożonych
│   │   ├── process.py          ← Ψ, ewaluacja procesu
│   │   └── queries.py          ← Q1, Q2, necessary/possibly
│   ├── parser/
│   │   ├── __init__.py
│   │   ├── lexer.py            ← tokenizer
│   │   ├── domain_parser.py    ← parsuje dziedzinę
│   │   └── query_parser.py     ← parsuje kwerendy
│   ├── examples/
│   │   ├── switches.txt
│   │   ├── ysp.txt
│   │   └── russian_turkey.txt
│   └── utils/
│       └── trace.py            ← formater "step-by-step trace"
├── gui/                        ← FRONTEND #1: Tkinter
│   ├── __init__.py
│   ├── app.py                  ← główne okno
│   ├── widgets.py              ← niestandardowe widgety (np. trace viewer)
│   └── themes.py               ← kolory, czcionki
├── web/                        ← FRONTEND #2 (opcjonalny, PK4)
│   ├── __init__.py
│   ├── server.py               ← Flask app
│   └── static/
│       ├── index.html
│       ├── app.js
│       └── style.css
└── tests/
    ├── test_formula.py
    ├── test_transitions.py
    ├── test_compound.py
    ├── test_queries.py
    ├── test_parser.py
    └── test_api.py             ← testy facade'a (kluczowe!)
```

## 3. Kluczowy kontrakt: `ds4/api.py`

Facade to **jedyny** sposób, w jaki frontend rozmawia z silnikiem. Wszystko serializowalne do JSON.

```python
# ds4/api.py

from typing import TypedDict, Literal

class Step(TypedDict):
    step_index: int                  # 0, 1, 2, ...
    compound_action: list[str]       # ["T1", "T2"]
    incoming_states: list[dict]      # [{"s1": True, "s2": True, "alarm": True}]
    outgoing_states: list[dict]      # po Res
    executable: bool                 # czy Res != 0 dla wszystkich incoming
    notes: list[str]                 # ["Akcja zlozona wykonalna", ...]

class SolveResult(TypedDict):
    ok: bool
    error: str | None                # tekst bledu parsowania, jesli ok=False
    sigma_size: int                  # |Σ|
    sigma0_size: int                 # |Σ_0|
    sigma: list[dict]                # wszystkie dopuszczalne stany
    sigma0: list[dict]               # stany poczatkowe
    query_kind: Literal["Q1_executable", "Q2_after"] | None
    quantifier: Literal["necessary", "possibly"] | None
    answer: bool | None              # wynik kwerendy
    trace: list[Step]                # krok po kroku
    final_states: list[dict]         # stany po wszystkich krokach
    goal_satisfied_by: list[dict]    # ktore final_states spelniaja gamma (Q2)

def solve(domain_text: str, query_text: str) -> SolveResult: ...

def validate_domain(domain_text: str) -> dict:
    """Zwraca {'ok': bool, 'errors': [...], 'fluents': [...], 'actions': [...]}."""
    ...

def list_examples() -> list[dict]:
    """Zwraca [{'id': 'switches', 'name': '...', 'description': '...'}, ...]."""
    ...

def load_example(example_id: str) -> dict:
    """Zwraca {'domain': '...', 'queries': ['...', '...']}."""
    ...
```

Każdy frontend (Tkinter, Flask) wywołuje **tylko** te funkcje. Żaden frontend nie importuje `ds4.engine.*` bezpośrednio.

## 4. Podział pracy — moduły = osoby

Każdy moduł ma **jednego właściciela** odpowiedzialnego za działanie + testy. Moduły komunikują się przez wąskie interfejsy.

| Moduł | Właściciel | Odpowiada za | Wejście | Wyjście |
|-------|------------|--------------|---------|---------|
| `engine/model.py` | **P4** | Klasy danych: `Fluent`, `Action`, `State`, `Domain`, `Statement` (i podtypy), `CompoundAction`, `Process` | — | klasy do importu |
| `engine/formula.py` | **P2** | AST formuł, parsowanie `"a and not b"`, `evaluate(formula, state)`, `to_dnf(formula)` | string formuły | bool / list literałów |
| `engine/states.py` | **P4** | `generate_sigma(domain)`, `generate_sigma0(domain, sigma)` | `Domain` | `list[State]` |
| `engine/transitions.py` | **P4** | `compute_res0`, `compute_new`, `compute_res` dla **akcji prostej** | `Action`, `State`, `Domain`, `Σ` | `set[State]` |
| `engine/compound.py` | **P5** | `affected_fluents`, `is_executable`, `compute_compound_res` dla 𝔸 | `CompoundAction`, `State`, `Domain`, `Σ` | `set[State]` |
| `engine/process.py` | **P5** | `execute_process` — iteracja Ψ, zwraca trace listy zbiorów osiągalnych | `Process`, `Σ_0`, `Domain`, `Σ` | `list[set[State]]` + trace |
| `engine/queries.py` | **P3** | `evaluate_q1_executable`, `evaluate_q2_after` × {necessary, possibly} | `Query`, trace | `bool` + uzasadnienie |
| `parser/lexer.py` | **P6** | tokenizer (regex) | string | `list[Token]` |
| `parser/domain_parser.py` | **P6** | parsuje plik dziedziny → `Domain` | string | `Domain` |
| `parser/query_parser.py` | **P6** | parsuje kwerendę → `Query` | string | `Query` |
| `ds4/api.py` | **P1** | facade `solve()`, `validate()`, `load_example()` — łączy wszystko | strings | `dict` (JSON-able) |
| `gui/app.py` | **P7** | okno Tkinter, layout, eventy | wywołuje `ds4.api` | UI |
| `gui/widgets.py` | **P7** | trace viewer, podświetlanie składni, examples picker | — | widgety |
| `examples/*.txt` | **P3** | 3 przykłady z kwerendami i oczekiwanymi odpowiedziami | — | pliki tekstowe |
| `tests/*.py` | wszyscy | każdy testuje swój moduł | — | pytest |

**Klucz do paralelizacji:** każdy może pracować niezależnie, mockując sąsiednie moduły. P7 nie czeka na P6 — używa hardcodowanej `Domain` z `model.py` do pisania GUI. P3 nie czeka na P5 — pisze kwerendy zakładając, że `process.execute_process` zwraca pewną strukturę.

## 5. Kontrakty między modułami (dataflow)

```
              parser              engine                       api          gui
                                                                            
domain_text ──> domain_parser ──> Domain ────────────────────> solve() ──> render
                                    │                            │
                                    ├── states.generate ──> Σ    │
                                    │                       Σ_0  │
                                    │                            │
                                    │   query_parser → Query     │
                                    │                            │
                                    └── transitions.compute_res ─┤
                                        compound.compute_res ────┤
                                        process.execute ─────────┤
                                        queries.evaluate ────────┤
                                                                 │
                                                     SolveResult ┘
                                                  (czysty dict)
```

## 6. UX — jak wygląda i jak się jej używa

### 6.1. Layout głównego okna (1100 × 750)

```
┌─────────────────────────────────────────────────────────────────────┐
│ DS4 Reasoner — Procesy działań złożonych                    _ □ ✕   │
├─────────────────────────────────────────────────────────────────────┤
│ [Plik ▼] [Przykłady ▼] [Pomoc ▼]                                    │
├──────────────────────────────────┬──────────────────────────────────┤
│ DZIEDZINA                        │ KWERENDA                          │
│ ┌──────────────────────────────┐ │ ┌──────────────────────────────┐ │
│ │ fluent s1                    │ │ │ necessary alarm after        │ │
│ │ fluent s2                    │ │ │   {T1, T2}                   │ │
│ │ fluent alarm                 │ │ │                              │ │
│ │ noninertial alarm            │ │ └──────────────────────────────┘ │
│ │ action T1                    │ │ [▶ Oblicz]   [Wyczyść]            │
│ │ action T2                    │ │                                   │
│ │                              │ ├──────────────────────────────────┤
│ │ initially s1 and s2          │ │ WYNIK                             │
│ │ always alarm <-> (s1 <-> s2) │ │ ┌──────────────────────────────┐ │
│ │ T1 causes not s1 if s1       │ │ │ Odpowiedź: TAK ✓             │ │
│ │ T1 causes s1 if not s1       │ │ │                              │ │
│ │ T2 causes not s2 if s2       │ │ │ Σ:  4 stany                  │ │
│ │ T2 causes s2 if not s2       │ │ │ Σ₀: 1 stan                   │ │
│ │                              │ │ │                              │ │
│ │                              │ │ │ ▼ Krok 1: {T1, T2}           │ │
│ │                              │ │ │   wejście: {s1,s2,alarm}     │ │
│ │                              │ │ │   wykonalna: TAK             │ │
│ │                              │ │ │   wyjście:  {¬s1,¬s2,alarm}  │ │
│ │                              │ │ │                              │ │
│ │                              │ │ │ Stany końcowe spełniające    │ │
│ │                              │ │ │ alarm: 1/1 ✓                 │ │
│ └──────────────────────────────┘ │ └──────────────────────────────┘ │
├──────────────────────────────────┴──────────────────────────────────┤
│ ● Gotowe · 4 stany · 1 stan początkowy · Czas: 12 ms                │
└─────────────────────────────────────────────────────────────────────┘
```

### 6.2. Komponenty UI

**Lewa strona (Dziedzina):**
- Edytor tekstowy z prostą podświetlaniem słów kluczowych (causes, releases, impossible, always, noninertial, initially, fluent, action) — kolor pomarańczowy
- Numerowanie linii
- Auto-walidacja w tle: po 500 ms od ostatniej zmiany wywołuje `api.validate_domain()` i podkreśla błędne linie na czerwono
- W stopce mini-info: "5 fluentów, 2 akcje, 6 zdań"

**Prawa góra (Kwerenda):**
- Mniejszy edytor jednoliniowy/wieloliniowy
- Dropdown z szablonami: "necessary executable...", "possibly ... after ...", itd.
- Przycisk **Oblicz** (główna akcja, niebieski/zielony)
- Skrót klawiszowy **Ctrl+Enter** = Oblicz

**Prawa dół (Wynik):**
- Górna sekcja "Odpowiedź" — wielką literą TAK/NIE z ikoną ✓/✗
- Sekcja "Stany" — `Σ` i `Σ₀` jako liczby + rozwijana lista (treeview)
- Sekcja "Trace" — accordion z każdym krokiem procesu:
  - rozwijalna sekcja per krok
  - wejście (zbiór stanów) → akcja → wyjście (zbiór stanów)
  - jeśli `Res = ∅` → czerwona ikonka i komentarz dlaczego (rozłączność / impossible / niespełniony warunek)
- Sekcja "Cel" (tylko Q2) — które stany końcowe spełniają γ

**Pasek statusu:**
- Status (Gotowe / Obliczam / Błąd)
- Liczba stanów / czas obliczeń
- Kropka zielona/czerwona

### 6.3. Pasek menu

| Menu | Akcje |
|------|-------|
| **Plik** | Nowy · Otwórz dziedzinę... · Zapisz dziedzinę... · Eksportuj wynik (TXT/JSON) · Wyjście |
| **Przykłady** | System przełączników · Yale Shooting · Rosyjska ruletka · (więcej...) |
| **Pomoc** | Składnia języka akcji · Składnia kwerend · O programie |

**Ważne:** "Przykłady" to killer-feature na demo — klik i prowadząca widzi wszystko skonfigurowane.

### 6.4. Flow użytkownika (happy path)

1. Otwiera aplikację → puste pola, status "Gotowe"
2. **Przykłady → System przełączników** → lewy panel wypełnia się dziedziną, prawy gornie pierwszą kwerendą
3. Klika **Oblicz** (lub Ctrl+Enter)
4. Wynik pojawia się w panelu prawym dolnym w <100 ms
5. Może przewinąć trace, rozwinąć kroki, zmienić kwerendę i odpalić ponownie

### 6.5. Flow błędu

1. Wpisuje błędną dziedzinę: `Tx causes alive if loaded` (akcja `Tx` nie zadeklarowana)
2. Walidacja w tle podkreśla linię na czerwono, w stopce: "❌ 1 błąd"
3. Klika Oblicz mimo to → panel wyniku pokazuje czerwoną ramkę z opisem: "Akcja 'Tx' nie jest zadeklarowana w dziedzinie (linia 7)"
4. Poprawia → status wraca na zielony

### 6.6. Estetyka

**Tkinter (stdlib):**
- Motyw `ttk` `clam` lub `vista` (na Windows `vista` wygląda OK)
- Czcionka kodowa: **Cascadia Mono / Consolas** dla edytorów, **Segoe UI** dla labelek
- Jasny motyw z jednym kolorem akcentowym (#2563eb — niebieski) dla przycisku Oblicz i podświetleń
- Dark mode opcjonalny (jeden checkbox w Pomoc)

**Jeśli CustomTkinter** — to samo, ale automatyczny dark/light + zaokrąglone rogi.

## 7. Dlaczego ta architektura wspiera webową wersję

Web wariant (PK4 lub demo bonusowe) wymaga **tylko**:

1. Plik `web/server.py` (~30 linii Flask):
   ```python
   from flask import Flask, request, jsonify
   from ds4 import api
   
   app = Flask(__name__, static_folder="static", static_url_path="")
   
   @app.post("/api/solve")
   def solve():
       data = request.json
       result = api.solve(data["domain"], data["query"])
       return jsonify(result)
   
   @app.get("/api/examples")
   def examples():
       return jsonify(api.list_examples())
   
   @app.get("/")
   def index():
       return app.send_static_file("index.html")
   ```

2. `static/index.html` + `app.js` z fetch `/api/solve` — to samo UX co w Tkinterze, tylko jako strona

3. `pip install flask` i `python main_web.py` → `localhost:5000`

**Nic w `ds4/` się nie zmienia.** To dosłownie dwa nowe pliki + opcjonalny endpoint. Dlatego facade `api.py` zwracający czyste `dict`-y jest tak ważny — to jest punkt rozdziału desktop/web.

## 8. Decyzje techniczne — co wybieramy

| Wybór | Decyzja | Dlaczego |
|-------|---------|----------|
| Język | Python 3.11+ | dataclasses, lepsze typing, match-case dla parsera |
| GUI primary | **Tkinter (stdlib)** | 0 zależności, najprostsze pakowanie, fallback bezpieczny |
| GUI alternatywa | CustomTkinter | jeśli wystarczy czasu pod koniec — drop-in replacement |
| Parser | ręczny rekurencyjny zstępujący | brak zależności (ANTLR/Lark to dodatkowe paczki); składnia jest prosta |
| Pakowanie | PyInstaller `--onefile --windowed` | jeden `.exe`, brak instalacji u użytkownika |
| Testy | pytest | standard de facto |
| Web (secondary) | Flask + vanilla JS | minimalne zależności; React niepotrzebny dla jednego formularza |
| Stan w GUI | trzymany w jednym dict-cie sesji | łatwy do serializacji (zapis/odczyt) |

## 9. Plan dzienny (07.05–11.05) — z przypisaniem osób

### Czw 07.05 — Fundament

| Osoba | Task | Deliverable |
|-------|------|-------------|
| P4 | `ds4/engine/model.py` (klasy danych) | importowalne klasy + 1 ręcznie złożona `Domain` w teście |
| P2 | `ds4/engine/formula.py` (parser formuł, evaluate) | `evaluate("a and not b", {a:1,b:0}) == True` |
| P4 | `ds4/engine/states.py` | `generate_sigma` na przykładzie przełączników → 4 stany |
| P1 | repo + `pyproject.toml` + szkielet `ds4/api.py` (stuby) | `pip install -e .` działa |
| P7 | szkielet `gui/app.py` (puste okno z layoutem) | uruchamia się, wyświetla 3 panele |

### Pią 08.05 — Akcje proste

| Osoba | Task |
|-------|------|
| P4 | `transitions.py`: Res₀, New, Res + minimalizacja |
| P4 | testy: YSP, Russian Turkey (Spin), system przełączników |
| P3 | szkic `examples/*.txt` z oczekiwanymi odpowiedziami |
| P7 | edytor + podświetlanie składni w `gui/widgets.py` |
| P6 | `parser/lexer.py` |

### Sob 09.05 — Złożone + procesy + kwerendy

| Osoba | Task |
|-------|------|
| P5 | `compound.py` (rozłączność, Res złożonej) |
| P5 | `process.py` (Ψ z trace) |
| P3 | `queries.py` (4 typy kwerend) |
| P6 | `parser/domain_parser.py` |
| P7 | trace viewer (accordion z krokami) |

### Nie 10.05 — Integracja

| Osoba | Task |
|-------|------|
| P1 | wypełnia `ds4/api.py` faktyczną logiką `solve()` |
| P6 | `parser/query_parser.py` |
| P7 | podpina GUI do `api.solve()`, format wyniku |
| P2/P3 | `examples/` finalne, każdy przykład z 2–3 kwerendami |
| wszyscy | end-to-end manual test każdego przykładu z GUI |

### Pon 11.05 — Pakowanie + demo

| Osoba | Task |
|-------|------|
| P1 | sanity check całości, scenariusz demo |
| P7 | `pyinstaller --onefile --windowed`, test na czystym Windowsie |
| P4/P5/P6 | bug fixes, polish |
| P2/P3 | przygotowanie prezentacji (slajdy / co pokazujemy) |

## 10. Ryzyka i kontrolki

| Ryzyko | Sygnał wczesny | Mitygacja |
|--------|----------------|-----------|
| Parser blokuje GUI | piątek wieczór, parser nie działa | GUI używa hardcoded `Domain` z `model.py` (P7 wpisze przykład wprost w kod). Demo bez parsera dalej możliwe |
| `.exe` nie startuje na obcej maszynie | niedziela test | `--onedir` zamiast `--onefile`, `--collect-all`, sprawdzić logi w `dist/main/main.log` |
| Brak Windows w zespole | znane od razu | GitHub Actions `windows-latest` buduje `.exe`, pobierany z artifacts |
| Złożoność `necessary` blokuje | sobota | implementacja przez śledzenie zbiorów `reachable[k]` i sprawdzanie czy każdy stan ma `Res ≠ ∅` — wystarcza dla 50% |
| Konflikt `release` + minimalizacja | testy nie przechodzą | unit testy z cheatsheet (rosyjska ruletka) jako benchmark; jeśli wynik się różni → bug |

## 11. Definicja "Gotowe" dla PK3

Aplikacja przeszła próbę:

1. ☐ `dist/DS4Reasoner.exe` istnieje i jest <50 MB
2. ☐ Kopiowany na czysty Windows odpala się dwuklikiem
3. ☐ Menu **Przykłady → System przełączników** wypełnia oba edytory
4. ☐ Klik **Oblicz** zwraca poprawny wynik w <500 ms
5. ☐ Trace pokazuje krok po kroku stany pośrednie
6. ☐ Wszystkie 3 przykłady (switches, YSP, Russian Turkey) działają end-to-end z minimum 2 kwerendami każdy
7. ☐ Błędna składnia → komunikat błędu, nie crash
8. ☐ Pytest: ≥30 testów jednostkowych zielone
9. ☐ README w `checkpoint3/` opisuje jak uruchomić, jak zbudować `.exe`
