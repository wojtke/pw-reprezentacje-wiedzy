# Projekt 4 — Procesy działań złożonych: kompletny cheatsheet

## Spis treści

1. [O co chodzi w projekcie](#1-o-co-chodzi-w-projekcie)
2. [Kluczowe pojęcia i akronimy](#2-kluczowe-pojęcia-i-akronimy)
3. [Warunki klasy DS4 (Z1–Z7)](#3-warunki-klasy-ds4-z1z7)
4. [Język akcji — składnia](#4-język-akcji--składnia)
5. [Semantyka — jak to działa pod spodem](#5-semantyka--jak-to-działa-pod-spodem)
6. [Akcje złożone i procesy](#6-akcje-złożone-i-procesy)
7. [Język kwerend — Q1 i Q2](#7-język-kwerend--q1-i-q2)
8. [Pełny przykład z rozwiązaniem krok po kroku](#8-pełny-przykład-z-rozwiązaniem-krok-po-kroku)
9. [Drugi przykład — niedeterminizm](#9-drugi-przykład--niedeterminizm)
10. [Trzeci przykład — niewykonalność i efekt pusty](#10-trzeci-przykład--niewykonalność-i-efekt-pusty)
11. [Algorytm implementacyjny (propozycja)](#11-algorytm-implementacyjny-propozycja)
12. [Skąd pochodzi wiedza — mapa wykładów](#12-skąd-pochodzi-wiedza--mapa-wykładów)
13. [Decyzje projektowe wykraczające poza wykłady](#13-decyzje-projektowe-wykraczające-poza-wykłady)
14. [Punkty niejasne / do wyjaśnienia z prowadzącą](#14-punkty-niejasne--do-wyjaśnienia-z-prowadzącą)

---

## 1. O co chodzi w projekcie

Celem jest zbudowanie dwóch rzeczy:

1. **Język akcji** — formalny język do *opisywania* dziedziny: jakie są zmienne (fluenty), jakie akcje, co te akcje robią, jakie są ograniczenia.
2. **Język kwerend** — formalny język do *zadawania pytań* o zachowanie systemu, np. „Czy ten ciąg akcji zawsze doprowadzi do celu?"

Oba języki dotyczą klasy **DS4** — oznaczenia użytego w opisie projektu na tę konkretną klasę systemów dynamicznych. Opis projektu nie rozszerza tego skrótu; wiemy jedynie, że jest to klasa zdefiniowana warunkami Z1–Z7.

DS4 łączy cechy kilku języków akcji z wykładów:

| Język | Co dodaje | Wykład |
|-------|-----------|--------|
| **A** (Action language A — najprostszy język akcji) | Determinizm, sekwencyjność, pełna informacja | Wykład 1 |
| **AR** (Action language with Ramifications — język akcji z ramifikacjami) | Niedeterminizm, skutki uboczne (ramifikacje), warunki integralności, niewykonalność | Wykład 2 |
| **AC** (Action language with Concurrency — język akcji współbieżnych) | Akcje złożone (wykonywane równolegle), konflikty, dekompozycja | Wykład 5 |

DS4 bierze z AR prawie wszystko (Z1–Z6), a z AC bierze pojęcie akcji złożonej, ale w **uproszczonej formie** — zamiast pełnego mechanizmu dekompozycji i dziedziczenia z AC, opis projektu wymaga jedynie warunku rozłączności zbiorów fluentów.

**Kluczowe ograniczenie z opisu projektu:** W opisach dziedzin (w zdaniach `causes`, `releases`, `impossible` itd.) występują **jedynie akcje proste**. Akcje złożone pojawiają się dopiero w języku kwerend (w procesach).

---

## 2. Kluczowe pojęcia i akronimy

### Pojęcia bazowe

| Pojęcie | Definicja | Przykład |
|---------|-----------|----------|
| **Fluent** (zmienna stanu, cecha świata) | Zmienna logiczna, która może mieć wartość *true* (1) lub *false* (0). Opisuje jakąś właściwość świata. | `loaded` (broń naładowana), `alive` (indyk żywy), `alarm` (alarm włączony) |
| **Literał** (literal — atom lub jego negacja) | Fluent `f` lub jego negacja `¬f`. | `loaded`, `¬alive` |
| **Stan** (state — pełne przypisanie wartości) | Odwzorowanie σ: F → {0, 1}. Przypisuje każdemu fluentowi wartość. | σ = {alive, ¬loaded} oznacza: indyk żywy, broń nienaładowana |
| **Formuła** (formula — wyrażenie zdaniowe) | Kombinacja fluentów ze spójnikami logicznymi: ¬, ∧, ∨, →, ↔ | `loaded ∧ alive`, `s₁ ↔ s₂` |
| **Akcja prosta** (atomic action — pojedyncze działanie) | Element zbioru A. Jedna niepodzielna czynność. W opisie dziedziny występują **wyłącznie** akcje proste. | `Load`, `Shoot`, `Toggle1` |
| **Akcja złożona** (compound action — zbiór akcji równoległych) | Zbiór 𝔸 = {A₁, …, Aₖ} ⊆ A. Akcje wykonywane jednocześnie. Występuje **tylko w języku kwerend** (w procesach). | {Toggle1, Toggle2} — naciskamy oba przełączniki naraz |
| **Proces** (process — ciąg kroków) | Ciąg P = (𝔸₁, …, 𝔸ₙ) akcji złożonych wykonywanych sekwencyjnie, n > 0. | ({Load}, {Shoot}) — najpierw ładujemy, potem strzelamy |
| **Dziedzina akcji** (action domain — opis świata) | Skończony zbiór D zdań języka akcji opisujących cały system. Zawiera tylko akcje proste. | Cały zestaw zdań: initially + causes + always + ... |
| **Sygnatura** (signature — „słownik" dziedziny) | Para Υ = (F, A), gdzie F to zbiór fluentów, A to zbiór akcji prostych. | Υ = ({alive, loaded}, {Load, Shoot}) |

### Pojęcia semantyczne

| Pojęcie | Definicja | Przykład |
|---------|-----------|----------|
| **Σ** (Sigma — zbiór stanów dopuszczalnych) | Wszystkie stany σ, które spełniają *każdy* warunek integralności (**always** α) z dziedziny D. Definicja z wykładu 2. | Jeśli `always (walking → alive)`, to stan {¬alive, walking} jest NIEdopuszczalny |
| **Σ₀** (Sigma-zero — zbiór stanów początkowych) | Podzbiór Σ wyznaczony przez zdania **initially** i **observable...after**. Może być wiele stanów początkowych (opis częściowy, Z7). **Uwaga:** na wykładach AR ma jeden σ₀ — rozszerzenie do zbioru Σ₀ wynika z potrzeb projektu, patrz sekcja 13. | `initially s₁` → Σ₀ = {σ ∈ Σ : σ(s₁) = 1} |
| **Res(A, σ)** (Result — zbiór stanów wynikowych) | Zbiór stanów, do których prowadzi wykonanie akcji A w stanie σ. Jeśli pusty → akcja niewykonalna. Definicja z wykładu 2. | Res(Shoot, {alive, loaded}) = {{¬alive, ¬loaded}} |
| **Res₀(A, σ)** (wstępny zbiór kandydatów) | Wszystkie stany spełniające efekty akcji A, których warunki wstępne zachodzą w σ. Przed minimalizacją. Wykład 2. | Może zawierać „za dużo" stanów |
| **New(A, σ, σ')** (zbiór zmian) | Zbiór literałów f̃ zachodzących w σ' takich, że: (a) f jest inercjalny i zmienił wartość, lub (b) f podlega **releases**. Wykład 2. | Jeśli σ = {alive, ¬loaded} i σ' = {alive, loaded}, to New = {loaded} |
| **Minimalizacja New** (zasada inercji) | Z Res₀ wybieramy tylko te σ', dla których New jest minimalny (w sensie inkluzji zbiorów). „Zmieniaj jak najmniej." Wykład 2. | Jeśli New(σ₁) = {f} i New(σ₂) = {f, g}, to zostawiamy tylko σ₁ |
| **Model** (model dziedziny D) | Struktura S spełniająca warunki (M1), (M2), (M3) — patrz sekcja 5. Wykład 2. | — |
| **Ramifikacja** (ramification — skutek uboczny) | Pośredni efekt akcji wynikający z warunków integralności, nie z samej akcji. Wykład 2. | Przełączenie s₁ zmienia alarm (bo `always alarm ↔ (s₁ ↔ s₂)`) |

### Pojęcia z kwerend

| Pojęcie | Definicja | Źródło |
|---------|-----------|--------|
| **necessary** (koniecznie — kwantyfikator „zawsze") | Dla **każdego** modelu, **każdego** stanu początkowego σ₀ ∈ Σ₀ i **każdej** ścieżki niedeterministycznej Ψ_S | Wykład 3 |
| **possibly** (możliwie — kwantyfikator „kiedykolwiek") | **Istnieje** stan początkowy i **istnieje** ścieżka spełniająca warunek | Wykład 3 |
| **executable** (wykonywalny) | Proces P jest wykonywalny, jeśli na każdym kroku zbiór Res jest niepusty | Wykład 3, rozszerzone o procesy w opisie projektu |

---

## 3. Warunki klasy DS4 (Z1–Z7)

### Z1. Prawo inercji

> Wartości fluentów nie zmieniają się, jeśli żadna akcja ich nie modyfikuje.

Implementowane przez **minimalizację zbioru New** (wykład 2, AR) — z kandydatów Res₀ wybieramy stany z *najmniejszą* liczbą zmian.

**Przykład:**
```
Fluenty: {f, g}
Akcja: A causes f

Stan: σ = {¬f, g}
Kandydaci Res₀: σ₁ = {f, g}, σ₂ = {f, ¬g}
  New(A, σ, σ₁) = {f}       ← zmienił się tylko f
  New(A, σ, σ₂) = {f, ¬g}   ← zmieniły się f i g
Res(A, σ) = {σ₁}            ← bo New(σ₁) ⊂ New(σ₂), minimalizacja
```

### Z2. Pełna informacja

> Znamy wszystkie fluenty F, wszystkie akcje A i wszystkie ich skutki.

Nie ma ukrytych akcji ani fluentów — system jest w pełni opisany przez dziedzinę D.

### Z3. Niedeterminizm działań

> Wykonanie akcji w danym stanie może prowadzić do więcej niż jednego stanu wynikowego.

Źródłem niedeterminizmu jest zdanie **releases** (wykład 2, AR) — mówi, że wartość fluenta *może, ale nie musi* się zmienić.

**Przykład (na podstawie Example 3.1 z wykładu 3):**
```
Toss releases heads

Stan: σ = {heads}
Po Toss: Res = {{heads}, {¬heads}}  ← dwa możliwe wyniki, jak rzut monetą
```

### Z4. Warunek wstępny i wynikowy

> Z każdą akcją związany jest warunek wstępny π i efekt α. Jeśli π nie jest spełniony, efekt jest pusty (stan się NIE zmienia, ale akcja JEST wykonana).

To jest inne niż niewykonalność (Z5)! Tutaj akcja się wykonuje, tylko nic nie robi.

**Przykład:**
```
Shoot causes ¬alive if loaded

Stan: σ = {alive, ¬loaded}  ← warunek "loaded" NIE jest spełniony
Po Shoot: Res = {σ}         ← stan się nie zmienia (efekt pusty), ale Shoot się wykonało
```

### Z5. Niewykonalność

> W pewnych stanach akcja może być niewykonalna — Res(A, σ) = ∅.

Opisywana zdaniem **impossible A if π** (wykład 2, AR). Różnica od Z4: tu akcja w ogóle *nie może się wykonać*.

**Przykład (na podstawie Example 2.3 z wykładu 2):**
```
impossible InsertCard if ¬hasCard

Stan: σ = {¬hasCard, ¬open}
Res(InsertCard, σ) = ∅      ← akcja niewykonalna, bo nie mamy karty
```

### Z6. Warunki integralności (ramifikacje)

> Zdania **always α** ograniczają zbiór stanów dopuszczalnych Σ i powodują *skutki uboczne* (ramifikacje).

Fluent **noninertial** (nieinercjalny) nie jest objęty minimalizacją zmian — jego wartość wynika z warunku integralności (wykład 2, AR, Example 2.4).

**Przykład (na podstawie Example 2.4 z wykładu 2):**
```
noninertial light
always light ↔ (switch₁ ↔ switch₂)

Przełączenie switch₁ automatycznie zmienia light (ramifikacja).
Nie trzeba pisać "Toggle1 causes ¬light" — wynika to z warunku always.
```

### Z7. Opis częściowy

> Dopuszczalny jest niepełny opis stanu początkowego i stanów wynikowych.

Jeśli `initially s₁` (tylko tyle), to nie wiemy jaki jest `s₂` — mamy **wiele** możliwych stanów początkowych. Kwantyfikacja kwerend musi to uwzględniać.

**Przykład:**
```
Fluenty: {s₁, s₂, alarm}
initially s₁                    ← wiemy tylko, że s₁ = true

Σ₀ = {σ ∈ Σ : σ(s₁) = 1}       ← może być wiele stanów początkowych
   = {{s₁, s₂, alarm}, {s₁, ¬s₂, ¬alarm}}  (jeśli always alarm ↔ (s₁ ↔ s₂))
```

---

## 4. Język akcji — składnia

Sygnatura: Υ = (F, A), gdzie:
- F — skończony zbiór **fluentów** (zmiennych stanu)
- A — skończony zbiór **akcji prostych**

Formuła α to dowolne wyrażenie zdaniowe nad F (wykład 2, AR):
```
α ::= f | ¬α | α ∧ β | α ∨ β | α → β | α ↔ β
```

### Zdania języka akcji (7 typów)

**Ważne:** Opis projektu mówi: *„W opisach dziedzin występują jedynie akcje proste."* Wszystkie poniższe zdania (i)–(iii) używają więc wyłącznie akcji prostych A ∈ A. Akcje złożone pojawiają się dopiero w kwerendach (procesach).

#### (i) Zdanie efektowe (effect statement) — wykład 1 (A), wykład 2 (AR)

```
A causes α if π
```

Znaczenie: Jeśli akcja prosta A jest wykonana w stanie spełniającym π, to stan wynikowy spełnia α.
Jeśli π ≡ ⊤ (tautologia), skracamy do `A causes α`.
Jeśli π nie jest spełniony → efekt jest pusty (Z4).

**Przykład (na podstawie Example 1.1 z wykładu 1 — Yale Shooting Problem):**
```
Load causes loaded                     ← bezwarunkowy efekt: po Load broń jest naładowana
Shoot causes ¬alive if loaded          ← warunkowy efekt: strzał zabija, JEŚLI broń naładowana
Shoot causes ¬loaded                   ← bezwarunkowy: strzał rozładowuje broń
```

#### (ii) Zdanie zwalniające (release statement) — wykład 2 (AR)

```
A releases f if π
```

Znaczenie: Po wykonaniu A w stanie spełniającym π, wartość fluenta f *może, ale nie musi* się zmienić. To jest źródło **niedeterminizmu** (Z3). Fluent f jest „zwolniony" z prawa inercji — trafia do zbioru New niezależnie od tego, czy faktycznie zmienił wartość.

**Przykład (na podstawie Example 2.2 z wykładu 2 — Russian Turkey):**
```
Spin releases loaded if loaded
← Po zakręceniu bębenkiem (rosyjska ruletka): broń może być naładowana albo nie.
```

#### (iii) Zdanie o niewykonalności (impossibility statement) — wykład 2 (AR)

```
impossible A if π
```

Znaczenie: Akcja prosta A jest *niewykonalna* w każdym stanie spełniającym π. Wtedy Res(A, σ) = ∅ (Z5).

**Uwaga:** W DS4 to zdanie dotyczy wyłącznie akcji prostych (wynika z opisu projektu). Aby uczynić pewną *kombinację* akcji niewykonalną w akcji złożonej, korzystamy z warunku rozłączności zbiorów fluentów — patrz sekcja 6.

**Przykład (na podstawie Example 2.3 z wykładu 2):**
```
impossible InsertCard if ¬hasCard
← Nie można włożyć karty, jeśli się jej nie ma.
```

#### (iv) Warunek integralności (constraint / always statement) — wykład 2 (AR)

```
always α
```

Znaczenie: Formuła α musi być spełniona w **każdym** dopuszczalnym stanie. Ogranicza Σ i generuje ramifikacje (Z6).

**Przykład (na podstawie Example 2.1 i 2.4 z wykładu 2):**
```
always walking → alive
← Kto chodzi, ten żyje. Stan {¬alive, walking} jest niedopuszczalny.

always alarm ↔ (s₁ ↔ s₂)
← Alarm włączony wtw, gdy oba przełączniki w tej samej pozycji.
```

#### (v) Fluent nieinercjalny (noninertial fluent specification) — wykład 2 (AR)

```
noninertial f
```

Znaczenie: Fluent f NIE podlega prawu inercji — nie jest minimalizowany w zbiorze New. Jego wartość wynika wyłącznie z warunków integralności (always).

**Przykład (na podstawie Example 2.4 z wykładu 2):**
```
noninertial light
always light ↔ (switch₁ ↔ switch₂)

← light nie jest minimalizowany — zmienia się automatycznie z warunkiem.
   Bez "noninertial" minimalizacja blokowałaby zmianę light
   (uznałaby ją za "niepotrzebną zmianę").
```

#### (vi) Obserwacja (value / observation statement) — wykład 2 (AR)

```
α after A₁, …, Aₙ               ← α zachodzi ZAWSZE po wykonaniu ciągu akcji
observable α after A₁, …, Aₙ     ← α MOŻE zachodzić po wykonaniu ciągu akcji
```

Służą do opisu **częściowej wiedzy** o stanach wynikowych (Z7).

**Przykład (na podstawie Example 1.2 z wykładu 1 — Stanford Murder Mystery):**
```
¬alive after Shoot
← Po strzale indyk na pewno nie żyje (obserwacja pewna).

observable loaded after Spin
← Po zakręceniu bębenkiem broń MOŻE być naładowana (obserwacja możliwa).
```

#### (vii) Stan początkowy (initial state statement) — wykład 1 (A), wykład 2 (AR)

```
initially α
```

Znaczenie: Opis (być może częściowy) stanu początkowego. Może być wiele takich zdań. Ich koniunkcja wyznacza Σ₀ (Z7).

**Przykład:**
```
initially alive
initially ¬loaded
← Stan początkowy: indyk żywy, broń nienaładowana.

initially s₁
← Wiemy tylko, że s₁ = true. Reszta nieznana → wiele stanów początkowych.
```

---

## 5. Semantyka — jak to działa pod spodem

### 5.1. Stany (wykład 1, 2)

**Stan** σ to odwzorowanie σ: F → {0, 1}.

Zapis: σ = {alive, ¬loaded} oznacza σ(alive) = 1, σ(loaded) = 0.

Piszemy σ ⊨ α gdy formuła α jest spełniona w stanie σ.

**Przykład z 3 fluentami {f, g, h}:**
Istnieje 2³ = 8 możliwych stanów:
```
σ₀ = {f, g, h}        σ₄ = {¬f, g, h}
σ₁ = {f, g, ¬h}       σ₅ = {¬f, g, ¬h}
σ₂ = {f, ¬g, h}       σ₆ = {¬f, ¬g, h}
σ₃ = {f, ¬g, ¬h}      σ₇ = {¬f, ¬g, ¬h}
```

### 5.2. Stany dopuszczalne Σ (wykład 2, AR)

Σ = { σ : dla każdego (always α) ∈ D, σ ⊨ α }

Warunki integralności filtrują stany — zostawiamy tylko te, które je spełniają.

**Przykład:**
```
Fluenty: {s₁, s₂, alarm}
always alarm ↔ (s₁ ↔ s₂)

Wszystkie stany (2³ = 8), ale dopuszczalne tylko 4:
  σ₀ = {s₁, s₂, alarm}        ← alarm ↔ (T ↔ T) = alarm ↔ T ✓
  σ₁ = {¬s₁, ¬s₂, alarm}      ← alarm ↔ (F ↔ F) = alarm ↔ T ✓
  σ₂ = {s₁, ¬s₂, ¬alarm}      ← ¬alarm ↔ (T ↔ F) = ¬alarm ↔ F ✓
  σ₃ = {¬s₁, s₂, ¬alarm}      ← ¬alarm ↔ (F ↔ T) = ¬alarm ↔ F ✓

Odrzucone np.:
  {s₁, s₂, ¬alarm} — bo s₁ ↔ s₂ = T, ale alarm = F → sprzeczne
```

### 5.3. Stany początkowe Σ₀

Σ₀ ⊆ Σ — podzbiór stanów dopuszczalnych spełniających zdania **initially** i **observable...after**.

**Uwaga:** Na wykładach (AR, wykład 2) struktura ma **jeden** stan początkowy σ₀. Rozszerzenie do **zbioru** Σ₀ jest konieczne z powodu Z7 (opis częściowy) — patrz sekcja 13.

**Przykład:**
```
initially s₁ ∧ s₂
Σ₀ = {σ ∈ Σ : σ ⊨ s₁ ∧ s₂} = {σ₀} = {{s₁, s₂, alarm}}

Ale gdybyśmy mieli tylko:
initially s₁                    ← opis CZĘŚCIOWY
Σ₀ = {σ₀, σ₂} = {{s₁, s₂, alarm}, {s₁, ¬s₂, ¬alarm}}
```

### 5.4. Funkcja przejścia — Res₀, New, Res (krok po kroku) — wykład 2 (AR)

To **serce** całego systemu. Dla akcji prostej A i stanu σ:

#### Krok 1: Res₀(A, σ) — kandydaci

Res₀(A, σ) = zbiór stanów σ' ∈ Σ takich, że:
- dla każdego zdania `(A causes α if π) ∈ D`: jeśli σ ⊨ π, to σ' ⊨ α

Innymi słowy: σ' musi spełniać WSZYSTKIE aktywne efekty akcji A (te, których warunek wstępny jest prawdziwy w σ).

**Przykład:**
```
Shoot causes ¬loaded
Shoot causes ¬alive if loaded

Stan: σ = {alive, loaded}

Aktywne efekty (warunki spełnione):
  - ¬loaded (bo zawsze aktywny)
  - ¬alive (bo σ ⊨ loaded)

Res₀(Shoot, σ) = {σ' ∈ Σ : σ' ⊨ ¬loaded ∧ ¬alive}
               = {{¬alive, ¬loaded}}
```

#### Krok 2: New(A, σ, σ') — co się zmieniło

Definicja z wykładu 2: New(A, σ, σ') to zbiór literałów f̃ takich, że **σ' ⊨ f̃** i zachodzi co najmniej jedno z:

1. f jest **inercjalny** (nie ma `noninertial f`) i σ(f) ≠ σ'(f), **LUB**
2. Istnieje zdanie `A releases f if π` w D takie, że σ ⊨ π

Kluczowe: patrzymy na **literał który zachodzi w σ'** (tj. f̃ to f lub ¬f, zależnie co jest prawdziwe w σ'), a warunki 1 i 2 dotyczą fluenta f.

**Przykład:**
```
Fluenty: {s₁, s₂, alarm}, alarm jest noninertial.
T₁ causes ¬s₁ if s₁

Stan: σ₀ = {s₁, s₂, alarm}
Kandydat: σ₃ = {¬s₁, s₂, ¬alarm}

New(T₁, σ₀, σ₃):
  - f = s₁: σ₃ ⊨ ¬s₁. Inercjalny, zmienił się (1→0) → ¬s₁ ∈ New  ✓
  - f = s₂: σ₃ ⊨ s₂. Inercjalny, NIE zmienił się (1→1) → nie wchodzi
  - f = alarm: σ₃ ⊨ ¬alarm. NONINERTIAL → nie wchodzi do New (nawet jeśli się zmienił!)

New(T₁, σ₀, σ₃) = {¬s₁}
```

#### Krok 3: Res(A, σ) — minimalizacja (prawo inercji)

Res(A, σ) = { σ' ∈ Res₀(A, σ) : nie istnieje σ'' ∈ Res₀(A, σ) taki, że New(A, σ, σ'') ⊂ New(A, σ, σ') }

Wybieramy TYLKO stany z **minimalnym** zbiorem New (w sensie inkluzji). „Zmieniaj jak najmniej!"

**Przykład (pełny, na podstawie Example 2.4 z wykładu 2):**
```
noninertial alarm
always alarm ↔ (s₁ ↔ s₂)
T₁ causes ¬s₁ if s₁
T₁ causes s₁ if ¬s₁

Stan: σ₀ = {s₁, s₂, alarm}
Aktywny efekt T₁: ¬s₁ (bo σ₀ ⊨ s₁)

Res₀(T₁, σ₀) = stany z Σ spełniające ¬s₁:
  σ₁ = {¬s₁, ¬s₂, alarm}    ← σ₁ ⊨ ¬s₁ ✓
  σ₃ = {¬s₁, s₂, ¬alarm}    ← σ₃ ⊨ ¬s₁ ✓

New(T₁, σ₀, σ₁) = {¬s₁, ¬s₂}    ← s₁ i s₂ się zmieniły (oba inercjalne)
New(T₁, σ₀, σ₃) = {¬s₁}          ← tylko s₁ się zmienił (alarm = noninertial, nie liczymy)

{¬s₁} ⊂ {¬s₁, ¬s₂}   → σ₃ jest lepszy (mniej zmian)

Res(T₁, σ₀) = {σ₃} = {{¬s₁, s₂, ¬alarm}}
```

### 5.5. Model dziedziny D (wykład 2, AR)

Na wykładach struktura w AR to S = (Σ, σ₀, Res). W projekcie, ze względu na Z7 (opis częściowy) i akcje złożone, naturalne rozszerzenie to S = (Σ, Σ₀, Res) — patrz sekcja 13.

Struktura jest **modelem** dziedziny D wtedy i tylko wtedy, gdy (wykład 2):

- **(M1)** Σ to zbiór WSZYSTKICH stanów spełniających KAŻDE `always α` z D
- **(M2)** Każde zdanie wartościujące (`initially`, `α after ...`, `observable α after ...`) jest prawdziwe w S
- **(M3)** Dla każdej akcji A i stanu σ, Res(A, σ) wyznaczone jest przez minimalizację New (jak powyżej)

---

## 6. Akcje złożone i procesy

### 6.1. Akcja złożona (opis projektu + wykład 5)

Akcja złożona 𝔸 = {A₁, …, Aₖ} ⊆ A to zbiór akcji prostych wykonywanych **jednocześnie** (współbieżnie).

Pojęcie akcji złożonej pochodzi z wykładu 5 (AC), ale w DS4 jest **uproszczone**: zamiast pełnego mechanizmu dekompozycji i dziedziczenia z AC, opis projektu wymaga jedynie spełnienia dwóch warunków.

#### Warunki wykonalności 𝔸 w stanie σ (opis projektu)

1. **Rozłączność:** Każde dwie składowe Aᵢ, Aⱼ ∈ 𝔸 (i ≠ j) wpływają na **rozłączne** zbiory fluentów w σ.
2. **Indywidualna wykonalność:** Każda składowa Aᵢ jest wykonalna w σ (Res(Aᵢ, σ) ≠ ∅).

Jeśli którykolwiek warunek nie jest spełniony → 𝔸 jest niewykonalna w σ.

**Uwaga:** W DS4 nie ma zdania `impossible` dla akcji złożonych (bo w dziedzinie są tylko akcje proste). Niemożliwość wykonania złożonej akcji wynika **wyłącznie** z: (a) naruszenia rozłączności, lub (b) niewykonalności którejś ze składowych prostych.

**Przykład — akcja wykonalna:**
```
T₁ causes ¬s₁ if s₁    ← T₁ wpływa na {s₁}
T₂ causes ¬s₂ if s₂    ← T₂ wpływa na {s₂}

Akcja złożona {T₁, T₂} w σ₀ = {s₁, s₂, alarm}:
  Zbiory fluentów: {s₁} ∩ {s₂} = ∅  ← rozłączne ✓
  Res(T₁, σ₀) ≠ ∅                   ✓
  Res(T₂, σ₀) ≠ ∅                   ✓
  → {T₁, T₂} jest wykonalna w σ₀
```

**Przykład — akcja NIEwykonalna (brak rozłączności):**
```
A causes f
B causes ¬f

Akcja złożona {A, B}:
  A wpływa na {f}, B wpływa na {f}
  {f} ∩ {f} = {f} ≠ ∅   ← NIE rozłączne
  → {A, B} jest niewykonalna (w każdym stanie)
```

#### Obliczanie Res(𝔸, σ) — propozycja projektowa

Opis projektu definiuje **warunki wykonalności** akcji złożonej, ale **nie definiuje** formalnie jak obliczać Res(𝔸, σ). Proponowane podejście (patrz sekcja 13):

1. Zbierz aktywne efekty **wszystkich** składowych Aᵢ w σ
2. Wyznacz Res₀(𝔸, σ) = stany spełniające łącznie wszystkie te efekty
3. Zastosuj minimalizację New (uwzględniając zmiany od wszystkich składowych)

Dzięki warunkowi rozłączności efekty składowych nie mogą być sprzeczne — każda składowa wpływa na inne fluenty.

**Przykład:**
```
T₁ causes ¬s₁ if s₁     T₂ causes ¬s₂ if s₂
noninertial alarm
always alarm ↔ (s₁ ↔ s₂)

σ₀ = {s₁, s₂, alarm}

Efekty składowe:
  T₁ wymusza ¬s₁
  T₂ wymusza ¬s₂

Szukamy σ' ∈ Σ: σ' ⊨ ¬s₁ ∧ ¬s₂
  σ₁ = {¬s₁, ¬s₂, alarm}  ← ¬s₁ ↔ ¬s₂ = T → alarm ✓

Res({T₁, T₂}, σ₀) = {σ₁} = {{¬s₁, ¬s₂, alarm}}
```

### 6.2. Procesy (opis projektu)

Proces P = (𝔸₁, …, 𝔸ₙ) to ciąg akcji złożonych wykonywanych **sekwencyjnie**, n > 0.

Zbiór stanów osiągalnych po procesie, startując ze stanu σ:

```
Ψ(ε, σ)                    = {σ}                    ← pusty ciąg, nic się nie dzieje
Ψ((𝔸₁, …, 𝔸ₖ), σ)         = ⋃_{σ' ∈ Ψ(prefix, σ)} Res(𝔸ₖ, σ')
```

Innymi słowy: wykonujemy krok po kroku, a na każdym kroku rozpatrujemy WSZYSTKIE możliwe stany z poprzedniego kroku.

Definicja Ψ* pochodzi z wykładu 1 (A) i 2 (AR), ale tam dotyczy ciągów akcji prostych. Rozszerzenie na ciągi akcji złożonych (procesy) wynika z opisu projektu.

**Przykład:**
```
Proces P = ({Load}, {Shoot})

Stan: σ = {alive, ¬loaded}

Krok 1: Res({Load}, σ) = {{alive, loaded}} = {σ₁}
Krok 2: Res({Shoot}, σ₁) = {{¬alive, ¬loaded}} = {σ₂}

Ψ(P, σ) = {σ₂} = {{¬alive, ¬loaded}}
```

---

## 7. Język kwerend — Q1 i Q2

Kwerendy dotyczą **procesów** (ciągów akcji złożonych). Opis projektu definiuje dwa typy kwerend.

### Q1 — Wykonywalność procesu (opis projektu)

| Kwerenda | Znaczenie | Warunek formalny |
|----------|-----------|-----------------|
| `necessary executable P` | P jest wykonywalny **zawsze** | ∀ σ₀ ∈ Σ₀, ∀ ścieżka Ψ_S: na każdym kroku procesu Res ≠ ∅ |
| `possibly executable P` | P jest wykonywalny **kiedykolwiek** | ∃ σ₀ ∈ Σ₀, ∃ ścieżka Ψ_S: na każdym kroku procesu Res ≠ ∅ |

Semantyka kwantyfikatorów necessary/possibly pochodzi z wykładu 3 (kwerendy executable). W DS4 rozszerzamy je o: (a) wiele stanów początkowych Σ₀, (b) procesy zamiast ciągów akcji prostych.

**Przykład:**
```
initially alive ∧ ¬loaded
impossible Shoot if ¬loaded

Proces P = ({Shoot})
  necessary executable P?
  → NIE, bo w σ₀ = {alive, ¬loaded} Shoot jest niewykonalny (impossible if ¬loaded)

Proces P = ({Load}, {Shoot})
  necessary executable P?
  → TAK, bo Load jest zawsze wykonywalny, a po Load broń jest loaded → Shoot wykonywalny
```

### Q2 — Osiągnięcie celu (opis projektu)

| Kwerenda | Znaczenie | Warunek formalny |
|----------|-----------|-----------------|
| `necessary γ after P` | P **zawsze** prowadzi do celu γ | ∀ σ₀ ∈ Σ₀, ∀ Ψ_S: jeśli P jest wykonywalny, to Ψ_S(P, σ₀) ⊨ γ |
| `possibly γ after P` | P **kiedykolwiek** prowadzi do γ | ∃ σ₀ ∈ Σ₀, ∃ Ψ_S: P jest wykonywalny i Ψ_S(P, σ₀) ⊨ γ |

**Przykład:**
```
initially alive ∧ ¬loaded
Load causes loaded
Shoot causes ¬loaded
Shoot causes ¬alive if loaded

Kwerenda: necessary ¬alive after ({Load}, {Shoot})?
  σ₀ = {alive, ¬loaded}
  Po Load: {alive, loaded}
  Po Shoot: {¬alive, ¬loaded}
  {¬alive, ¬loaded} ⊨ ¬alive ✓
  Odpowiedź: TAK

Kwerenda: necessary alive after ({Shoot})?
  σ₀ = {alive, ¬loaded}
  Po Shoot: {alive, ¬loaded}  ← efekt pusty, bo ¬loaded
  {alive, ¬loaded} ⊨ alive ✓
  Odpowiedź: TAK (broń nienaładowana → strzał nic nie robi → indyk żyje)
```

---

## 8. Pełny przykład z rozwiązaniem krok po kroku

### System bezpieczeństwa

**Fluenty:** F = {s₁, s₂, alarm}
**Akcje:** A = {T₁, T₂}

**Dziedzina D:**
```
noninertial alarm
initially s₁ ∧ s₂
always alarm ↔ (s₁ ↔ s₂)
T₁ causes ¬s₁ if s₁
T₁ causes s₁ if ¬s₁
T₂ causes ¬s₂ if s₂
T₂ causes s₂ if ¬s₂
```

**Krok 1: Stany dopuszczalne Σ**

Wszystkie 2³ = 8 stanów. Filtrujemy warunkiem `always alarm ↔ (s₁ ↔ s₂)`:

| Stan | s₁ | s₂ | alarm | s₁ ↔ s₂ | alarm ↔ (s₁ ↔ s₂) | Dopuszczalny? |
|------|----|----|-------|---------|-------------------|---------------|
| σ₀ | 1 | 1 | 1 | T | T ↔ T = T | ✓ |
| σ₁ | 0 | 0 | 1 | T | T ↔ T = T | ✓ |
| σ₂ | 1 | 0 | 0 | F | F ↔ F = T | ✓ |
| σ₃ | 0 | 1 | 0 | F | F ↔ F = T | ✓ |
| — | 1 | 1 | 0 | T | F ↔ T = F | ✗ |
| — | 0 | 0 | 0 | T | F ↔ T = F | ✗ |
| — | 1 | 0 | 1 | F | T ↔ F = F | ✗ |
| — | 0 | 1 | 1 | F | T ↔ F = F | ✗ |

**Σ = {σ₀, σ₁, σ₂, σ₃}**

**Krok 2: Stany początkowe Σ₀**

`initially s₁ ∧ s₂` → Σ₀ = {σ ∈ Σ : σ ⊨ s₁ ∧ s₂} = {σ₀}

**Krok 3: Obliczanie Res dla akcji prostych**

**Res(T₁, σ₀)** gdzie σ₀ = {s₁, s₂, alarm}:
```
Aktywny efekt: T₁ causes ¬s₁ if s₁  → σ₀ ⊨ s₁ ✓ → efekt: ¬s₁
(T₁ causes s₁ if ¬s₁ → σ₀ ⊭ ¬s₁ → NIEaktywny)

Res₀(T₁, σ₀) = {σ' ∈ Σ : σ' ⊨ ¬s₁} = {σ₁, σ₃}

New(T₁, σ₀, σ₁) — σ₁ = {¬s₁, ¬s₂, alarm}:
  s₁: inercjalny, 1→0 → ¬s₁ ∈ New
  s₂: inercjalny, 1→0 → ¬s₂ ∈ New
  alarm: NONINERTIAL, pomijamy
  New = {¬s₁, ¬s₂}

New(T₁, σ₀, σ₃) — σ₃ = {¬s₁, s₂, ¬alarm}:
  s₁: inercjalny, 1→0 → ¬s₁ ∈ New
  s₂: inercjalny, 1→1 → nie zmienił się
  alarm: NONINERTIAL, pomijamy
  New = {¬s₁}

Minimalizacja: {¬s₁} ⊂ {¬s₁, ¬s₂}
Res(T₁, σ₀) = {σ₃}
```

**Kompletna tabela Res dla wszystkich par (akcja, stan):**

| | σ₀ = {s₁,s₂,a} | σ₁ = {¬s₁,¬s₂,a} | σ₂ = {s₁,¬s₂,¬a} | σ₃ = {¬s₁,s₂,¬a} |
|---|---|---|---|---|
| **Res(T₁, ·)** | {σ₃} | {σ₂} | {σ₁} | {σ₀} |
| **Res(T₂, ·)** | {σ₂} | {σ₃} | {σ₀} | {σ₁} |

**Krok 4: Kwerenda 1 — `necessary ¬alarm after ({T₁})`**

```
Σ₀ = {σ₀}
Res({T₁}, σ₀) = Res(T₁, σ₀) = {σ₃}
σ₃ = {¬s₁, s₂, ¬alarm}
σ₃ ⊨ ¬alarm ✓

Dla każdego σ₀ ∈ Σ₀ i każdej ścieżki: wynik spełnia ¬alarm.
Odpowiedź: TAK ✓
```

**Krok 5: Kwerenda 2 — `necessary alarm after ({T₁, T₂})`**

```
Akcja złożona {T₁, T₂} w σ₀:
  T₁ wpływa na {s₁}, T₂ wpływa na {s₂}
  {s₁} ∩ {s₂} = ∅ → rozłączne ✓
  Res(T₁, σ₀) ≠ ∅ ✓, Res(T₂, σ₀) ≠ ∅ ✓
  → wykonalna

Efekty: T₁ wymusza ¬s₁, T₂ wymusza ¬s₂
Szukamy σ' ∈ Σ: σ' ⊨ ¬s₁ ∧ ¬s₂
  σ₁ = {¬s₁, ¬s₂, alarm} → ¬s₁ ∧ ¬s₂ ✓

Res({T₁, T₂}, σ₀) = {σ₁}
σ₁ ⊨ alarm ✓

Odpowiedź: TAK ✓
```

---

## 9. Drugi przykład — niedeterminizm

### Rosyjska ruletka (na podstawie Example 2.2 z wykładu 2)

**Fluenty:** F = {alive, loaded}
**Akcje:** A = {Load, Spin, Shoot}

**Dziedzina D:**
```
initially alive ∧ ¬loaded
Load causes loaded
Shoot causes ¬loaded
Shoot causes ¬alive if loaded
Spin releases loaded if loaded       ← źródło niedeterminizmu!
```

**Stany:**
```
Σ = {σ₀, σ₁, σ₂, σ₃}  (brak warunków always → wszystkie stany dopuszczalne)
σ₀ = {alive, ¬loaded}    σ₁ = {alive, loaded}
σ₂ = {¬alive, ¬loaded}   σ₃ = {¬alive, loaded}
```

**Σ₀ = {σ₀}**

**Kluczowa akcja — Spin w σ₁ = {alive, loaded}:**
```
Aktywne efekty: BRAK zdań "causes" dla Spin
Zdanie zwalniające: Spin releases loaded if loaded → σ₁ ⊨ loaded ✓

Res₀(Spin, σ₁) = Σ = {σ₀, σ₁, σ₂, σ₃}  (brak efektów do wymuszenia)

Obliczanie New — dla każdego fluenta sprawdzamy: czy jest inercjalny i zmienił się,
lub czy podlega releases. Literał wchodzący do New to ten, który zachodzi w σ'.

New(Spin, σ₁, σ₀):  σ₀ = {alive, ¬loaded}
  f = alive: σ₁(alive)=1, σ₀(alive)=1 → nie zmienił się, brak releases → nie wchodzi
  f = loaded: σ₁(loaded)=1, σ₀(loaded)=0 → zmienił się (inercjalny) → ¬loaded ∈ New
              + releases aktywny → też wpada
  New = {¬loaded}

New(Spin, σ₁, σ₁):  σ₁ = {alive, loaded}
  f = alive: nie zmienił się, brak releases → nie wchodzi
  f = loaded: nie zmienił się, ALE releases aktywny → loaded ∈ New
  New = {loaded}

New(Spin, σ₁, σ₂):  σ₂ = {¬alive, ¬loaded}
  f = alive: zmienił się (1→0), inercjalny → ¬alive ∈ New
  f = loaded: zmienił się + releases → ¬loaded ∈ New
  New = {¬alive, ¬loaded}

New(Spin, σ₁, σ₃):  σ₃ = {¬alive, loaded}
  f = alive: zmienił się (1→0), inercjalny → ¬alive ∈ New
  f = loaded: nie zmienił się, ALE releases aktywny → loaded ∈ New
  New = {¬alive, loaded}

Minimalizacja:
  New(σ₀) = {¬loaded}
  New(σ₁) = {loaded}
  New(σ₂) = {¬alive, ¬loaded}    ← {¬loaded} ⊂ {¬alive, ¬loaded} → eliminacja
  New(σ₃) = {¬alive, loaded}     ← {loaded} ⊂ {¬alive, loaded} → eliminacja

  {¬loaded} i {loaded} — żadne nie jest podzbiorem drugiego → oba zostają

Res(Spin, σ₁) = {σ₀, σ₁}    ← NIEDETERMINIZM: broń naładowana LUB nienaładowana
```

**Kwerenda: `possibly ¬alive after ({Load}, {Spin}, {Shoot})`**
```
σ₀ → Load → σ₁ → Spin → {σ₀, σ₁}
  ścieżka 1: σ₀ → Shoot → σ₀ (efekt pusty, bo ¬loaded) → alive ← nie osiąga celu
  ścieżka 2: σ₁ → Shoot → σ₂ = {¬alive, ¬loaded} → ¬alive ← osiąga cel!

Istnieje ścieżka → possibly: TAK
```

**Kwerenda: `necessary ¬alive after ({Load}, {Spin}, {Shoot})`**
```
Nie każda ścieżka prowadzi do ¬alive (ścieżka 1: alive).
Odpowiedź: NIE
```

---

## 10. Trzeci przykład — niewykonalność i efekt pusty

### Otwieranie drzwi (na podstawie Example 2.3 z wykładu 2)

**Fluenty:** F = {open, hasCard}
**Akcje:** A = {InsertCard, Break}

**Dziedzina D:**
```
initially ¬open
initially ¬hasCard
InsertCard causes open
impossible InsertCard if ¬hasCard    ← Z5: niewykonalność
Break causes open if ¬open           ← Z4: warunkowy efekt
```

**Kwerenda: `necessary executable ({InsertCard})`**
```
σ₀ = {¬open, ¬hasCard}
σ₀ ⊨ ¬hasCard → impossible InsertCard → Res(InsertCard, σ₀) = ∅
Odpowiedź: NIE (akcja niewykonalna)
```

**Kwerenda: `necessary executable ({Break})`**
```
σ₀ = {¬open, ¬hasCard}
Break causes open if ¬open → σ₀ ⊨ ¬open → efekt: open
Res(Break, σ₀) = {{open, ¬hasCard}} ≠ ∅
Odpowiedź: TAK
```

**Kwerenda: `necessary executable ({Break}, {Break})`** (proces: Break, potem znowu Break)
```
Krok 1: Res(Break, σ₀) = {{open, ¬hasCard}} = {σ₁}
Krok 2: σ₁ = {open, ¬hasCard}
  Break causes open if ¬open → σ₁ ⊭ ¬open → efekt PUSTY (Z4)
  Res(Break, σ₁) = {σ₁}  ← stan się nie zmienia, ale akcja się wykonała!
  Res ≠ ∅ → wykonywalny
Odpowiedź: TAK (ale drugie Break nic nie robi — efekt pusty ≠ niewykonalność)
```

---

## 11. Algorytm implementacyjny (propozycja)

**Uwaga:** Poniższy algorytm jest propozycją implementacyjną — nie pochodzi bezpośrednio z wykładów ani z opisu projektu. Patrz sekcja 13.

```
WEJŚCIE: Dziedzina D, Kwerenda Q

1. PARSOWANIE
   Wczytaj D → wyodrębnij: F, A, zdania causes/releases/impossible/always/
                            noninertial/initially/observable/after

2. WYZNACZANIE Σ (stanów dopuszczalnych)
   Generuj wszystkie 2^|F| stanów
   Σ ← {σ : ∀(always α) ∈ D, σ ⊨ α}

3. WYZNACZANIE Σ₀ (stanów początkowych)
   Σ₀ ← {σ ∈ Σ : ∀(initially α) ∈ D, σ ⊨ α}
   Uwzględnij zdania "observable α after ..." i "α after ..."

4. DLA KAŻDEJ akcji prostej A ∈ A i stanu σ ∈ Σ:
   a) Res₀(A, σ):
      jeśli ∃(impossible A if π) ∈ D taki, że σ ⊨ π → Res₀ = ∅, KONIEC
      efekty ← {α : (A causes α if π) ∈ D ∧ σ ⊨ π}
      Res₀ ← {σ' ∈ Σ : ∀α ∈ efekty, σ' ⊨ α}

   b) New(A, σ, σ') dla każdego σ' ∈ Res₀:
      N ← ∅
      dla każdego f ∈ F:
        f̃ ← literał zachodzący w σ' (f lub ¬f)
        jeśli (f inercjalny ∧ σ(f) ≠ σ'(f)) LUB (∃(A releases f if π) ∈ D ∧ σ ⊨ π):
          N ← N ∪ {f̃}
      zwróć N

   c) Res(A, σ):
      Res ← {σ' ∈ Res₀ : nie ∃σ'' ∈ Res₀ t.że New(σ'') ⊂ New(σ')}

5. DLA AKCJI ZŁOŻONEJ 𝔸 = {A₁, …, Aₖ} w stanie σ:
   a) Wyznacz zbiory fluentów wpływanych przez każdą Aᵢ w σ
   b) Sprawdź rozłączność parami → jeśli nie → Res(𝔸, σ) = ∅
   c) Sprawdź ∀i: Res(Aᵢ, σ) ≠ ∅ → jeśli nie → Res(𝔸, σ) = ∅
   d) Zbierz aktywne efekty od wszystkich składowych
   e) Res₀(𝔸, σ) ← stany spełniające łącznie wszystkie efekty
   f) Zastosuj minimalizację New

6. DLA PROCESU P = (𝔸₁, …, 𝔸ₙ):
   Dla necessary: śledzić WSZYSTKIE ścieżki (drzewa) od KAŻDEGO σ₀ ∈ Σ₀
   Dla possibly: wystarczy znaleźć JEDNĄ ścieżkę od JAKIEGOŚ σ₀ ∈ Σ₀

   Uproszczona wersja (zbiory stanów osiągalnych):
   reachable ← Σ₀
   dla k = 1, …, n:
     next ← ∅
     dla każdego σ ∈ reachable:
       next ← next ∪ Res(𝔸ₖ, σ)
     reachable ← next
     jeśli reachable = ∅ → proces niewykonalny na kroku k

7. ODPOWIEDŹ NA KWERENDĘ:
   Q1-necessary executable P:
     ∀σ₀ ∈ Σ₀, ∀ ścieżki: na każdym kroku Res(𝔸ₖ, σ) ≠ ∅
   Q1-possibly executable P:
     ∃σ₀ ∈ Σ₀, ∃ ścieżka: na każdym kroku Res(𝔸ₖ, σ) ≠ ∅
   Q2-necessary γ after P:
     ∀σ₀ ∈ Σ₀, ∀ stany końcowe σ_end osiągalne z σ₀: σ_end ⊨ γ
   Q2-possibly γ after P:
     ∃σ₀ ∈ Σ₀, ∃ stan końcowy σ_end osiągalny z σ₀: σ_end ⊨ γ

   WAŻNE: "uproszczona wersja" z kroku 6 (zbiór reachable) nie rozróżnia
   ścieżek — wystarczy do possibly, ale dla necessary trzeba
   śledzić, że Res jest niepusty DLA KAŻDEGO stanu w reachable
   na KAŻDYM kroku (a nie tylko że zbiór next jest niepusty).
```

---

## 12. Skąd pochodzi wiedza — mapa wykładów

| Pojęcie/mechanizm | Wykład | Język | Uwagi |
|---|---|---|---|
| Stan, fluent, literał, formuła | 1 | A | Fundament |
| Zdanie efektowe `A causes α if π` | 1, 2 | A, AR | Identyczne w projekcie |
| Funkcja przejścia Ψ | 1 | A | W projekcie rozszerzona na procesy |
| Prawo inercji (warunek M2 w A) | 1 | A | W AR/DS4: minimalizacja New |
| Zdanie zwalniające `A releases f if π` | 2 | AR | Źródło niedeterminizmu |
| Niewykonalność `impossible A if π` | 2 | AR | Res = ∅ |
| Warunek integralności `always α` | 2 | AR | Filtruje Σ, generuje ramifikacje |
| Fluent nieinercjalny `noninertial f` | 2 | AR | Wyłączony z minimalizacji New |
| Obserwacje `observable α after ...` | 2 | AR | Opis częściowy (Z7) |
| Res₀, New, Res (minimalizacja) | 2 | AR | Kluczowy algorytm |
| Model (M1, M2, M3) | 2 | AR | Warunki poprawności |
| Kwerendy necessary/possibly (wartościowe) | 3 | AR | Rozszerzone o procesy w DS4 |
| Kwerendy executable | 3 | AR | Rozszerzone o procesy w DS4 |
| Kwerendy accessible, goal | 3 | AR | Opis projektu ich nie wymaga |
| AQ (kwalifikacje z constraints) | 3 | AQ | Wariant — odrzucanie stanów po minimalizacji |
| ARQ (preserves) | 3 | ARQ | Wariant mieszany |
| Akcja złożona (compound action) | 5 | AC | W DS4 uproszczona (rozłączność zamiast dekompozycji) |
| Konflikty między akcjami | 5 | AC | W DS4 zastąpione warunkiem rozłączności |
| Dekompozycja akcji | 5 | AC | W DS4 NIE potrzebna (rozłączność eliminuje potrzebę) |
| Dziedziczenie efektów | 5 | AC | W DS4 nie potrzebne (rozłączność = brak interakcji) |

### Co z wykładów 3 i 4 jest potencjalnie przydatne, ale nie wymagane

- **AQ/ARQ (wykład 3):** Opis projektu mówi, że *warunki integralności wpływają na skutki uboczne akcji* (Z6), co odpowiada podejściu AR (ramifikacje). Nie jest jasne, czy potrzebny jest też mechanizm AQ (kwalifikacje) — patrz sekcja 14.
- **AD (wykład 4):** Efekty typowe/atypowe. Opis projektu ich nie wymaga.
- **AL (wykład 4):** Scenariusze z czasem liniowym. Opis projektu ich nie wymaga.

---

## 13. Decyzje projektowe wykraczające poza wykłady

Poniższe elementy **nie pochodzą bezpośrednio z wykładów** ani z opisu projektu — są koniecznymi decyzjami projektowymi, które trzeba podjąć i uzasadnić.

### 13.1. Zbiór stanów początkowych Σ₀ zamiast jednego σ₀

**Na wykładach (AR, wykład 2):** Struktura to S = (Σ, **σ₀**, Res) — z jednym stanem początkowym.

**W projekcie (Z7):** Opis jest częściowy → wiele możliwych stanów początkowych. Konieczne jest rozszerzenie do S = (Σ, **Σ₀**, Res), gdzie Σ₀ = {σ ∈ Σ : σ spełnia wszystkie zdania `initially` i jest zgodny z obserwacjami}.

**Konsekwencja:** Kwerendy `necessary` kwantyfikują po **wszystkich** σ₀ ∈ Σ₀, a `possibly` szukają **jakiegoś** σ₀ ∈ Σ₀. Na wykładach, gdzie jest jeden σ₀, ta kwantyfikacja jest trywialna.

### 13.2. Struktura S = (Σ, Σ₀, Res) z Res na 2^A

**Na wykładach (AR, wykład 2):** Res: A × Σ → 2^Σ — domena to akcje proste.

**W projekcie:** Potrzebujemy Res dla akcji złożonych. Naturalny typ: Res: 2^A × Σ → 2^Σ (jak w AC, wykład 5).

### 13.3. Obliczanie Res(𝔸, σ) dla akcji złożonych

Opis projektu definiuje **kiedy** akcja złożona jest wykonalna (rozłączność + indywidualna wykonalność), ale **nie definiuje formalnie** jak obliczać wynik Res(𝔸, σ). Trzeba to zaprojektować.

Proponowane podejście: skoro zbiory fluentów są rozłączne, efekty składowych nie mogą wchodzić w konflikt. Łączymy je w koniunkcję i stosujemy standardową minimalizację New.

### 13.4. Algorytm implementacyjny

Cały pseudokod z sekcji 11 jest propozycją — wykłady definiują semantykę (co jest modelem), ale nie podają algorytmu implementacyjnego.

### 13.5. Rozróżnienie necessary vs possibly w implementacji

Na wykładach definicje necessary/possibly są czysto semantyczne (kwantyfikatory). Implementacja wymaga decyzji, jak efektywnie przeglądać drzewo ścieżek — np. czy śledzić pełne drzewo czy zbiory osiągalnych stanów.

---

## 14. Punkty niejasne / do wyjaśnienia z prowadzącą

### 14.1. Czy Z6 oznacza podejście AR (ramifikacje) czy AQ (kwalifikacje)?

Opis projektu: *„Warunki integralności wpływają na skutki uboczne akcji."*

- W AR (wykład 2): warunki integralności (`always`) ograniczają Σ **przed** minimalizacją New → generują ramifikacje (skutki uboczne).
- W AQ (wykład 3): stany niedopuszczalne odrzucane **po** minimalizacji → mogą powodować niewykonalność akcji (kwalifikacje).

Sformułowanie „wpływają na skutki uboczne" sugeruje AR, ale warto potwierdzić.

### 14.2. Czy opis częściowy (Z7) dotyczy tylko stanu początkowego?

Opis projektu: *„Dopuszczalny jest opis częściowy zarówno stanu początkowego, jak i pewnych stanów wynikających z wykonań sekwencji akcji."*

Opis częściowy stanu początkowego → Σ₀ jako zbiór.
Ale co oznacza „opis częściowy stanów wynikających"? Czy chodzi o zdania `observable α after A₁,...,Aₙ`? Czy te zdania dotyczą ciągów akcji prostych, czy mogą dotyczyć procesów (ciągów akcji złożonych)?

### 14.3. Jak definiować „zbiory fluentów, na które wpływa akcja"?

Warunek rozłączności wymaga określenia, na jakie fluenty wpływa akcja A w stanie σ. Opis projektu tego nie precyzuje.

Możliwe definicje:
- **Wąska:** Fluenty jawnie wymienione w aktywnych zdaniach `causes` i `releases` dla A w σ.
- **Szeroka:** Fluenty z wąskiej definicji + fluenty zmieniane pośrednio przez ramifikacje.

### 14.4. Efekt pusty (Z4) vs niewykonalność (Z5) — interakcja

Jeśli akcja ma kilka zdań `causes` i żaden warunek wstępny nie jest spełniony → efekt jest pusty (Z4), stan się nie zmienia, ale akcja jest wykonana (Res = {σ}).

Ale jeśli jednocześnie jest zdanie `impossible A if π` z π spełnionym → Res = ∅ (Z5).

Co ma priorytet? Naturalna interpretacja: `impossible` ma priorytet — jeśli akcja jest niemożliwa, to nawet pusty efekt nie zachodzi.

### 14.5. Czy kwerendy Q1/Q2 wymagają kwantyfikacji po modelach?

Na wykładach (wykład 3) kwerendy kwantyfikują *po modelach*: „D ⊨ Q iff **dla każdego modelu** S of D...". Jeśli dziedzina jest dobrze zdefiniowana (spójna) i nie ma niedospecyfikowania poza Z7, to model jest wyznaczony jednoznacznie (z dokładnością do σ₀).

Pytanie: czy w DS4 wystarczy kwantyfikować po Σ₀ (stanach początkowych) wewnątrz jednego modelu, czy trzeba rozpatrywać wiele modeli?
