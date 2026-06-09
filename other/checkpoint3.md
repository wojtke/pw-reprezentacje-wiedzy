# Checkpoint 3 — Postęp w aplikacji (Project 4)

**Deadline:** 12.05.2026 (wtorek, traktowany jak piątek), slot Projekt 4: **15:45 – 16:00** [H‑12]
**Format:** wstępna prezentacja aplikacji (15 min). Wymagane jest **min. 50%** prac aplikacyjnych. [R‑3]
**Założenie:** część teoretyczna jest gotowa; ten checkpoint dotyczy wyłącznie działającej implementacji.

---

## Czego oczekuje prowadząca na CP3

- Działająca, **nie-konsolowa** aplikacja (GUI) — finalnie ma być EXE pod Windows; na CP3 wystarczy działający prototyp z GUI. [H‑13]
- Demo pokazujące, że język akcji + język kwerend są wczytywane, parsowane i odpowiadają na pytania Q1/Q2. [P4]
- Co najmniej jeden kompletny scenariusz domeny (np. cowboy/duel, pralka, korki — cokolwiek z części teoretycznej) załadowany jako przykład. [SUM‑7]
- Krótki opis stanu prac, co działa, co zostało do CP4 (3.06/11.06). [H‑14]

---

## Co aplikacja musi umieć (zakres minimalny dla DS4)

Projekt nr 4 dotyczy klasy systemów dynamicznych **DS4** o własnościach Z1–Z7: [P4]

- Z1: prawo inercji,
- Z2: pełna informacja o `F`, `A` i skutkach akcji,
- Z3: niedeterminizm,
- Z4: każda akcja ma warunek wstępny i warunek wynikowy (formuły zdaniowe); przy niespełnionym warunku wstępnym efekt jest pusty,
- Z5: w pewnych stanach akcja może być niewykonalna,
- Z6: warunki integralności wpływają na skutki uboczne (ramification),
- Z7: dopuszczalny opis **częściowy** stanu początkowego i stanów wynikowych.

### Język opisu akcji (action language) — dla akcji **prostych** [P4]
Aplikacja musi przyjąć od użytkownika definicję dziedziny zawierającą:

- zbiór fluentów `F`,
- zbiór akcji prostych `A`,
- warunki integralności (formuły zdaniowe),
- statementy postaci (zgodnie z wykładem A/AR):
  - `A causes α if π` (warunek wynikowy `α` przy preconditionie `π`),
  - `impossible A if π` (zakaz wykonania — czyli skrót, nie osobny typ), [SUM‑2]
  - `initially α` (opis stanu początkowego, częściowy dozwolony).
- niedeterminizm: dla jednej akcji można podać **wiele** alternatywnych klauzul `causes`, dających różne stany wynikowe. [P4 Z3]

### Język kwerend — dla akcji **złożonych** i procesów [P4]
- Akcja złożona `A = {A1, …, Ak} ⊆ A` (zbiór akcji prostych).
- `A` jest wykonalna w stanie `σ` wtw:
  1. każde dwie różne `Ai, Aj ∈ A` wpływają w `σ` na **rozłączne** zbiory zmiennych,
  2. wszystkie składowe `Ai` są wykonalne w `σ`.
- Proces `P = (A1, …, An)`, `n > 0` — ciąg akcji złożonych.

### Zapytania, na które aplikacja MUSI odpowiadać [P4 Q1‑Q2]
- **Q1.** Czy proces `P` jest **zawsze / kiedykolwiek** wykonalny ze stanu początkowego `σ₀`?
- **Q2.** Czy proces `P` **zawsze / kiedykolwiek** prowadzi do osiągnięcia celu `γ` (formuła zdaniowa) ze stanu początkowego `σ₀`?

„Zawsze” = we wszystkich modelach i dla wszystkich stanów początkowych spełniających opis częściowy oraz dla wszystkich gałęzi niedeterminizmu. [SUM‑4]
„Kiedykolwiek” = istnieje model, stan początkowy i ścieżka, dla której warunek zachodzi. [SUM‑4]

---

## Funkcjonalność aplikacji do pokazania na CP3

Minimalny, demonstrowalny przepływ (wynika z [P4] + wymogu „min. 50% prac" [R‑3]):

1. **Wczytanie / edycja dziedziny** (z pliku lub w GUI):
   - lista fluentów, akcji, warunków integralności, statementów `causes` / `initially`.
2. **Generowanie modelu** systemu dynamicznego (stany jako wartościowania `F`, relacja `Res` dla każdej akcji prostej).
   - obsługa częściowego stanu początkowego → zbiór `σ₀` możliwych stanów, [P4 Z7]
   - obsługa niedeterminizmu → wiele następników dla jednej pary `(σ, A)`, [P4 Z3]
   - filtrowanie przez warunki integralności. [P4 Z6]
3. **Definicja procesu** `P = (A1, …, An)` w GUI (każde `Ai` jako zbiór akcji prostych). [P4]
4. **Sprawdzenie wykonalności akcji złożonej** — weryfikacja warunku rozłączności wpływów + wykonalność składowych. [P4]
5. **Odpowiedź na Q1 i Q2** w wariantach `always` / `ever`, z możliwością podejrzenia ścieżki/kontrprzykładu. [P4]
6. Co najmniej **1 przykładowa dziedzina** zaszyta jako demo do prezentacji. [SUM‑7]

---

## Status wymagany na 12.05 (≥ 50% aplikacji) [R‑3]

Na dzień prezentacji powinny działać:

- [ ] GUI (nie konsola) — okno z edytorem dziedziny i wynikami. [H‑13]
- [ ] Parser języka akcji (fluenty, akcje, `causes`, `initially`, `impossible`, integrity constraints). [P4]
- [ ] Generacja modelu / wszystkich stanów (z obsługą częściowego `σ₀`). [P4 Z7]
- [ ] Wyznaczanie `Res(A, σ)` dla akcji prostej z uwzględnieniem niedeterminizmu i ramification. [P4 Z3, Z6]
- [ ] Sprawdzanie wykonalności **akcji złożonej** (rozłączność wpływów + wykonalność składowych). [P4]
- [ ] Wykonanie procesu `P` krok po kroku. [P4]
- [ ] Q1 (`always` / `ever` executable) — działające. [P4 Q1]
- [ ] Q2 (`always` / `ever` reaches `γ`) — działające albo wyraźnie zaplanowane do CP4. [P4 Q2]
- [ ] 1–2 wbudowane przykłady do demo. [SUM‑7]

Co może jeszcze nie być zrobione (ale do CP4 musi):
- pełny zestaw testów (każdy członek zespołu min. 4–5 testów), [R‑3]
- dokumentacja techniczna, instrukcja użytkownika, [R‑3] [H‑13]
- estetyka GUI, paczka Windows EXE, [H‑13]
- 4–5 przykładów z dokładnymi obliczeniami w dokumentacji. [R‑3]

---

## Co przygotować na sam slot 15 min [H‑12]

1. Krótki slajd / okno: temat (Projekt 4 — Procesy działań złożonych), zespół, podział pracy.
2. Live demo:
   - załaduj przykładową dziedzinę,
   - pokaż wygenerowany model,
   - zdefiniuj akcję złożoną i pokaż test wykonalności,
   - zdefiniuj proces i odpowiedz na Q1 oraz Q2 w wariantach `always`/`ever`.
3. Slajd „co zostało do CP4" + harmonogram do 3.06/11.06. [H‑14]

---

## Źródła

Oznaczenia w nawiasach kwadratowych odsyłają do następujących dokumentów:

- **[P4]** — `sem01/rw/pro/docs/Projekt_nr_4___2026.pdf` (treść zadania nr 4 „Procesy działań złożonych"):
  - klasa **DS4** i własności **Z1–Z7** (s. 1, l. 6–15),
  - definicja akcji prostej i akcji złożonej + warunek rozłącznych wpływów (s. 1, l. 16–21),
  - definicja procesu `P = (A1, …, An)` (s. 1, l. 22),
  - **Q1** wykonalność procesu always/ever (s. 1, l. 27),
  - **Q2** osiągnięcie celu `γ` always/ever (s. 1, l. 28–29).

- **[H‑12], [H‑13], [H‑14]** — `sem01/rw/pro/docs/RW_01___HANDOUT.pdf` (Lecture 1 / Regulamin + Punkty kontrolne):
  - **[H‑12]** slajd 12 — „Punkt kontrolny 3: Postęp w aplikacji", data **12.05.2026**, Projekt 4 slot **15:45–16:00**,
  - **[H‑13]** slajd 13 — „Punkt kontrolny 4: Prezentacja aplikacji", aplikacja **nie może być konsolowa**, EXE pod Windows + komplet dokumentacji,
  - **[H‑14]** slajd 14 — terminy CP4 (3.06 / 11.06.2026).

- **[R‑3]** — `sem01/rw/regulamin.pdf` (Regulamin przedmiotu, pkt 3 „Szczegółowe sposoby etapowej weryfikacji…", s. 1, l. 18–28):
  - „W trakcie 1‑szego i **3‑ciego** punktu kontrolnego studenci przedstawiają **postępy** w przygotowaniach… **wymagane jest wykonanie min. 50% prac**",
  - dla 3. PK przedmiotem prezentacji jest **aplikacja**,
  - dla 4. PK: dokumentacja techniczna, podręcznik użytkownika, testy (każdy uczestnik **4–5 testów**), opis wkładu osobowego.

- **[SUM‑2], [SUM‑4], [SUM‑7]** — `sem01/rw/rw-summary.md` (uwagi po CP2, kontekst dot. semantyki i prezentacji):
  - **[SUM‑2]** pkt 2 — `impossible` i `initial` to **skróty**, nie osobne typy statementów,
  - **[SUM‑4]** pkt 4 — semantyka **necessary/possible** przy wielu modelach i wielu stanach początkowych (definicje „always"/„ever" użyte powyżej),
  - **[SUM‑7]** pkt 7 — wymóg **konkretnych przykładów** (np. cowboy/duel) ilustrujących definicje i działanie.
