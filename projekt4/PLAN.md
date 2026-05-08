# Plan — Projekt 4: Procesy działań złożonych

## PK1 — 26.03.2026 ✅

- Wstępna teoria (checkpoint1)
  - Język akcji DS4: składnia (causes, releases, impossible, always, noninertial, initially, after, observable)
  - Semantyka akcji prostych (Res₀, New, minimalizacja)
  - Konflikty (Przypadek 1: causes–causes, Przypadek 2: causes–releases)
  - Dekompozycje akcji złożonych (D1/D2/D3)
  - Procesy, funkcja Ψ
  - Język kwerend (necessary/possibly × executable/after)
  - 2 przykłady z obliczeniami (YSP + Producent/konsumenci)

## PK2 — 23.04.2026

**Co to jest:** Prezentacja opracowania pełnej części teoretycznej + złożenie dokumentu LaTeX (wydruk + wersja elektroniczna).

### Wymagania z regulaminu (dosłownie)

> W trakcie drugiego punktu kontrolnego studenci prezentują opracowanie części
> teoretycznej projektu oraz składają dokumentację tej części zadania (wersja
> drukowana i papierowa, przygotowany w systemie LaTeX) zawierającą:
> **(a) szczegółowy opis języka reprezentacji dziedzin i języka kwerend,**
> **(b) 4-5 przykładów z dokładnymi obliczeniami.**

### In scope (PK2)

1. **Język akcji** — pełna składnia (BNF) + opis każdego z 8 typów zdań dziedziny
2. **Semantyka** — formalne definicje: stany, Σ, Σ₀, Res₀, New, minimalizacja, impossible, konflikty, dekompozycje, Res akcji złożonej
3. **Procesy** — definicja procesu, funkcja Ψ, model dziedziny
4. **Język kwerend** — pełna składnia (BNF) + formalna semantyka 4 typów kwerend (necessary/possibly × executable/after), relacja D ⊨ Q
5. **4–5 przykładów z dokładnymi obliczeniami** — każdy: sygnatura → dziedzina → Σ → Σ₀ → proces → tabele Res krok-po-kroku → kwerendy z odpowiedziami
6. **Decyzje projektowe** — uzasadnienie odchyleń od treści zadania (AC zamiast rozłączności, Σ₀ zamiast σ₀, priorytet impossible)

### Out of scope (PK2)

- Implementacja / kod źródłowy → PK3
- Dokumentacja techniczna programu → PK4
- Podręcznik użytkownika → PK4
- Testy aplikacji → PK4
- Opis wkładu uczestników → PK4

### Format / objętość

- LaTeX, wydruk + wersja elektroniczna
- Orientacyjnie **15–25 stron** (checkpoint1/PK1 miała 6 stron, PK2 = pełne rozwinięcie)
- Sekcja algorytmizacji (pseudokod) opcjonalna, ale przydatna jako pomost do PK3

### Rozdziały checkpoint2.tex

1. **Wprowadzenie** (1.1 Cel projektu, 1.2 Klasa DS4 — Z1–Z7, 1.3 Mapa zapożyczeń z wykładów, 1.4 Ograniczenia DS4)
2. **Podstawy formalne** (2.1 Sygnatura, 2.2 Formuły zdaniowe, 2.3 Stany, 2.4 Literały i DNF)
3. **Język akcji — składnia** (3.1 Tabela zbiorcza 8 typów zdań, 3.2 Pełna BNF, 3.3 Opis poszczególnych zdań, 3.4 Dziedzina D)
4. **Semantyka — akcje proste** (4.1 Σ, 4.2 Σ₀, 4.3 Res₀, 4.4 New, 4.5 Minimalizacja Res(A,σ), 4.6 Niewykonalność)
5. **Semantyka — akcje złożone** (5.1 Definicja 𝔸, 5.2 Konflikty, 5.3 Dekompozycje, 5.4 Res(𝔸,σ), 5.5 Restricted inheritance, 5.6 Porównanie z treścią projektu)
6. **Procesy i model dziedziny** (6.1 Proces P, 6.2 Funkcja Ψ, 6.3 Model M1/M2/M3, 6.4 Jednoznaczność modelu)
7. **Język kwerend** (7.1 Składnia BNF, 7.2 Semantyka formalna 4 typów, 7.3 Interakcja wykonalności/celu, 7.4 Relacja D ⊨ Q)
8. **Algorytmizacja** (8.1 Generacja Σ, 8.2 Res akcji prostej, 8.3 Wykrywanie konfliktów, 8.4 Generacja dekompozycji, 8.5 Res akcji złożonej i procesu, 8.6 Ewaluacja kwerend, 8.7 Złożoność)
9. **Decyzje projektowe** (9.1 Σ₀ vs σ₀, 9.2 AC vs rozłączność, 9.3 Priorytet impossible, 9.4 Kwerendy a niewykonalność, 9.5 Kwantyfikacja po modelach)
10. **Przykłady** (10.1 YSP z rosyjską ruletką, 10.2 Producent i konsumenci, 10.3 Dwa przełączniki i alarm, 10.4 Scenariusz integrujący)
11. **Podsumowanie i kierunek dalszych prac**

- Literatura + Appendix A (opcj. pełne tabele Res)

## PK3 — 12.05.2026

Prezentacja prototypu aplikacji (min. 50% gotowej).
Aplikacja musi być plikiem wykonywalnym Windows — **nie może** być konsolowa (wymóg z regulaminu).

### Decyzje technologiczne

**Język i środowisko:** Python 3.11+
- Szybki development, dobrze znany w zespole
- Biblioteka GUI: **CustomTkinter** (nowoczesny wygląd, łatwy w użyciu) lub PyQt6
- Pakowanie do .exe: **PyInstaller** (`pyinstaller --onefile --windowed main.py`)

**Struktura katalogów:**
```
checkpoint3/
├── main.py                  ← punkt wejścia (uruchamia GUI)
├── engine/
│   ├── __init__.py
│   ├── model.py             ← struktury danych (Fluent, Action, State, Statement, ...)
│   ├── formula.py           ← ewaluacja formuł, DNF
│   ├── states.py            ← generacja Σ, Σ₀
│   ├── transitions.py       ← Res₀, New, Res dla akcji prostych
│   ├── compound.py          ← Res dla akcji złożonych, sprawdzanie rozłączności
│   ├── process.py           ← Ψ (ewaluacja procesu)
│   └── queries.py           ← Q1/Q2, necessary/possibly
├── parser/
│   ├── __init__.py
│   ├── lexer.py             ← tokenizer
│   └── parser.py            ← parser zdań dziedziny i kwerend
├── gui/
│   ├── __init__.py
│   └── app.py               ← okno główne, edytor, wyniki
├── tests/
│   ├── test_transitions.py
│   ├── test_compound.py
│   └── test_queries.py
└── examples/
    ├── switches.txt         ← system bezpieczeństwa (PK1/PK2)
    ├── ysp.txt              ← Yale Shooting Problem
    └── russian_turkey.txt   ← rosyjska ruletka
```

### Plan dzienny (07.05–11.05)

#### Dzień 1 — Czwartek 07.05: Fundament silnika

**Cel:** Działające generowanie Σ i Σ₀ + ewaluacja formuł

1. **`engine/model.py`** — struktury danych:
   - `State` — frozenset literałów; metody: `satisfies(formula)`, `as_dict()`
   - `Domain` — zbiory fluentów F, akcji A, lista Statement
   - Typy zdań: `CausesStmt`, `ReleasesStmt`, `ImpossibleStmt`, `AlwaysStmt`,
     `NonInertialStmt`, `InitiallyStmt`, `ObservableAfterStmt`, `AfterStmt`

2. **`engine/formula.py`** — rekurencyjny ewaluator formuł zdaniowych:
   - Parsowanie wewnętrzne (prefix/AST) lub wyrażenia Python z `eval` + whitelist
   - `evaluate(formula, state) → bool`
   - `to_dnf(formula) → list[frozenset]` (do wykrywania konfliktów w PK4 — na razie opcjonalne)

3. **`engine/states.py`**:
   - `generate_sigma(domain) → list[State]` — wszystkie 2^|F| stany filtrowane przez `always`
   - `generate_sigma0(domain, sigma) → list[State]` — filtrowanie przez `initially` i `observable`

4. **Test manualny:** System przełączników (F={s₁,s₂,alarm}, `always alarm ↔ (s₁ ↔ s₂)`, `initially s₁ ∧ s₂`)
   → Σ powinno mieć 4 stany, Σ₀ = {σ₀ = {s₁,s₂,alarm}}

---

#### Dzień 2 — Piątek 08.05: Res dla akcji prostych

**Cel:** Pełna implementacja Res(A, σ) z minimalizacją New

1. **`engine/transitions.py`**:
   - `compute_res0(action, state, domain, sigma) → set[State]`
     - Sprawdź `impossible A if π` — jeśli aktywne → zwróć ∅
     - Zbierz aktywne efekty `causes α if π`
     - Zwróć `{σ' ∈ Σ : σ' ⊨ all active effects}`
   - `compute_new(action, state, successor, domain) → frozenset[literal]`
     - Dla każdego fluenta f: sprawdź warunki (inercjalny+zmienił się) LUB (releases aktywne)
     - Literał = f jeśli σ'(f)=1, ¬f jeśli σ'(f)=0
   - `compute_res(action, state, domain, sigma) → set[State]`
     - Filtruj Res₀ — zostaw tylko stany z minimalnym New (brak nadrzędnego podzbioru)

2. **Testy jednostkowe** (`tests/test_transitions.py`):
   - YSP: `Res(Shoot, {alive, loaded})` → `{{¬alive, ¬loaded}}`
   - System przełączników: `Res(T₁, {s₁,s₂,alarm})` → `{{¬s₁, s₂, ¬alarm}}`
   - Efekt pusty: `Res(Shoot, {alive, ¬loaded})` → `{{alive, ¬loaded}}`
   - Niewykonalność: `Res(InsertCard, {¬hasCard, ¬open})` → `∅`
   - Niedeterminizm: `Res(Spin, {alive, loaded})` → `{{alive, loaded}, {alive, ¬loaded}}`

---

#### Dzień 3 — Sobota 09.05: Akcje złożone + procesy + kwerendy

**Cel:** Pełna ewaluacja kwerend Q1/Q2

1. **`engine/compound.py`** — Res dla akcji złożonej 𝔸 = {A₁,…,Aₖ}:
   - `get_affected_fluents(action, state, domain) → set[str]`
     — fluenty jawnie wymienione w aktywnych `causes`/`releases` dla A w stanie σ
   - `is_compound_executable(compound, state, domain, sigma) → bool`
     — sprawdź rozłączność parami i indywidualną wykonalność
   - `compute_compound_res(compound, state, domain, sigma) → set[State]`
     — zbierz efekty wszystkich składowych, wyznacz Res₀, zastosuj minimalizację New
     (gwarantowana spójność dzięki rozłączności)

2. **`engine/process.py`**:
   - `execute_process(process, initial_states, domain, sigma) → list[set[State]]`
     — zwraca listę zbiorów osiągalnych stanów po każdym kroku
     — `reachable[0] = Σ₀`, `reachable[k] = ⋃_{σ ∈ reachable[k-1]} Res(𝔸ₖ, σ)`
   - Funkcja pomocnicza: `is_step_executable(compound, states, domain, sigma) → bool`
     — czy Res ≠ ∅ dla KAŻDEGO stanu w zbiorze (necessary) lub JAKIEGOŚ (possibly)

3. **`engine/queries.py`** — 4 typy kwerend:
   ```
   Q1 necessary executable P:
     ∀ σ₀ ∈ Σ₀: na każdym kroku Res(𝔸ₖ, σ₀_path) ≠ ∅ dla każdej ścieżki
     → implementacja: śledź reachable krok po kroku;
       jeśli na JAKIMŚ kroku istnieje σ ∈ reachable z Res(𝔸ₖ,σ)=∅ → NIE

   Q1 possibly executable P:
     ∃ ścieżka od jakiegoś σ₀ ∈ Σ₀: wszystkie kroki mają Res ≠ ∅
     → reachable = DFS/BFS po drzewie ścieżek; jeśli jakiś liść jest wykonalny → TAK

   Q2 necessary γ after P:
     ∀ σ₀ ∈ Σ₀, ∀ stan końcowy σ_end osiągalny: σ_end ⊨ γ
     → wszystkie stany w reachable[n] spełniają γ (i reachable[n] ≠ ∅)

   Q2 possibly γ after P:
     ∃ σ_end osiągalny ze stanu σ₀ ∈ Σ₀: σ_end ⊨ γ
     → któryś stan w reachable[n] spełnia γ
   ```

4. **Testy integracyjne** — 3 przykłady z cheatsheet:
   - System przełączników: `necessary alarm after ({T₁,T₂})` → TAK
   - YSP: `necessary ¬alive after ({Load},{Shoot})` → TAK
   - Rosyjska ruletka: `possibly ¬alive after ({Load},{Spin},{Shoot})` → TAK,
     `necessary ¬alive after ({Load},{Spin},{Shoot})` → NIE

---

#### Dzień 4 — Niedziela 10.05: Parser + GUI szkielet

**Cel:** Działający parser + minimalne okno GUI

1. **`parser/lexer.py`** — tokenizer (regex-based):
   ```
   Tokeny: KEYWORD (causes, releases, impossible, always, noninertial,
                    initially, observable, after, necessary, possibly,
                    executable, fluent, action),
           IDENT, NOT (¬ lub "not"), AND (∧ lub "and"), OR (∨ lub "or"),
           IMPLIES (→ lub "->"), IFF (↔ lub "<->"),
           LBRACE ({), RBRACE (}), COMMA, NEWLINE/SEMICOLON
   ```

2. **`parser/parser.py`** — parser rekurencyjny zstępujący:
   - Składnia wejściowa (uproszczona, ASCII-friendly):
     ```
     fluent alive
     fluent loaded
     action Load
     action Shoot
     noninertial alarm
     always alarm <-> (s1 <-> s2)
     initially alive and not loaded
     Load causes loaded
     Shoot causes not loaded
     Shoot causes not alive if loaded
     impossible Shoot if not loaded    (opcjonalnie)
     ```
   - Kwerendy:
     ```
     necessary executable {Load}, {Shoot}
     possibly not alive after {Load}, {Shoot}
     ```
   - Parser produkuje obiekty z `engine/model.py`

3. **`gui/app.py`** — okno CustomTkinter:
   - Lewy panel: pole tekstowe "Dziedzina" (edytowalny)
   - Prawy panel górny: pole "Kwerenda"
   - Prawy panel dolny: pole "Wynik" (read-only)
   - Przyciski: "Oblicz", "Wyczyść", "Załaduj przykład" (dropdown)
   - Pasek statusu: "Gotowy" / "Błąd parsowania" / "Obliczam..."

---

#### Dzień 5 — Poniedziałek 11.05: Integracja + packaging + demo

**Cel:** Działający .exe z demo

1. **Integracja end-to-end** (`main.py`):
   - GUI wywołuje parser → parser produkuje Domain + Query → silnik oblicza → wynik do GUI
   - Obsługa błędów parsowania (wyświetl komunikat w panelu wyniku)

2. **Przykłady** (pliki `.txt` do załadowania):
   - `examples/switches.txt` — system przełączników + 3 kwerendy
   - `examples/ysp.txt` — Yale Shooting Problem + 2 kwerendy
   - `examples/russian_turkey.txt` — rosyjska ruletka + kwerendy possibly/necessary

3. **Pakowanie do .exe:**
   ```bash
   pip install pyinstaller customtkinter
   pyinstaller --onefile --windowed --name DS4Reasoner main.py
   # → dist/DS4Reasoner.exe
   ```

4. **Scenariusz demo (15 min):**
   1. Otwarcie aplikacji (pokaż że to .exe, nie konsola)
   2. Załaduj przykład: system przełączników
   3. Oblicz `necessary alarm after {T1, T2}` → TAK, wytłumacz krok po kroku
   4. Wpisz ręcznie `possibly ¬alarm after {T1, T2}` → NIE (bo always → zawsze alarm)
   5. Załaduj YSP, oblicz `necessary not alive after {Load}, {Shoot}` → TAK
   6. Załaduj rosyjską ruletkę, oblicz `possibly`/`necessary` ¬alive → TAK/NIE

### Co liczy się jako "50% gotowe"

Minimalne wymagania spełnione na PK3:

| Komponent | Wymagany? | Status cel |
|-----------|-----------|------------|
| Generacja Σ i Σ₀ | ✅ TAK | done |
| Res dla akcji prostych (causes, impossible) | ✅ TAK | done |
| Res z releases (niedeterminizm) | ✅ TAK | done |
| Res dla akcji złożonych (rozłączność) | ✅ TAK | done |
| Ewaluacja procesu Ψ | ✅ TAK | done |
| Kwerendy Q1/Q2 (wszystkie 4 typy) | ✅ TAK | done |
| Parser dziedziny | ✅ TAK | done (ASCII) |
| Parser kwerend | ✅ TAK | done (ASCII) |
| Okno GUI (nie konsolowe!) | ✅ TAK | done |
| Plik .exe | ✅ TAK | done |
| Walidacja wejść / komunikaty błędów | ❌ PK4 | — |
| Wizualizacja grafu stanów | ❌ PK4 | — |
| Zdania `observable α after` | ⚠️ opcjonalne | — |

### Ryzyka i mitygacje

| Ryzyko | Mitygacja |
|--------|-----------|
| Parser zbyt skomplikowany → opóźnienie | Zacznij od ręcznego tworzenia Domain w Pythonie; parser dodaj później |
| PyInstaller nie buduje .exe na macOS → brak Windows | Zbuduj na Windows (VM lub zdalnie przez GitHub Actions / classmate) |
| Semantyka Q1 necessary za złożona | Implementacja przez śledzenie zbiorów reachable jest wystarczająca dla 50% |
| Brak czasu na GUI | Tkinter (stdlib) zamiast CustomTkinter — prostszy, zero instalacji |

### Podział pracy PK3

| Osoba | Zadanie | Dni |
|-------|---------|-----|
| P4 | `engine/model.py`, `engine/states.py`, `engine/transitions.py` | Dn1–Dn2 |
| P5 | `engine/compound.py`, `engine/process.py` | Dn2–Dn3 |
| P6 | `engine/queries.py`, `parser/` | Dn3–Dn4 |
| P7 | `gui/app.py`, `examples/`, PyInstaller packaging | Dn4–Dn5 |
| P2/P3 | `engine/formula.py`, testy jednostkowe | Dn1–Dn3 |
| P1 | Integracja, `main.py`, scenariusz demo | Dn5 |

## PK4 — 11.06.2026

Finalne złożenie (prezentacja + wydruk + wersja elektroniczna).

- Dokumentacja
  - Zaktualizowana część teoretyczna (z uwzględnieniem uwag z PK2)
  - Dokumentacja techniczna programu
  - Podręcznik użytkownika
- Testy
  - Min. 4–5 testów na osobę (7 osób → ≥28 testów)
  - Różnorodne scenariusze (każdy mechanizm DS4)
- Opis wkładu każdego uczestnika
- Dopracowanie aplikacji
  - Obsługa edge case'ów
  - Walidacja danych wejściowych
