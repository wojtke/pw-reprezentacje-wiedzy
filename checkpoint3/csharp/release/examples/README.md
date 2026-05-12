# DS4 - przykłady demonstracyjne z wyjaśnieniami

Ten folder zawiera małe przykłady do pokazania działania reasonera DS4.

Każdy przykład składa się z czterech plików:

```text
nazwa.domain
nazwa.query
nazwa.expected
nazwa.explanation.md
```

Znaczenie plików:

```text
.domain          - opis dziedziny
.query           - kwerenda do sprawdzenia
.expected        - oczekiwany wynik, true albo false
.explanation.md  - dokładne wytłumaczenie przykładu
```

## Kolejność polecana do prezentacji

1. `demo_01_conditional_cause_possibly_q` - prosty efekt warunkowy i `possibly`.
2. `demo_02_conditional_cause_necessary_q_false` - ta sama domena, ale `necessary` daje NIE.
3. `demo_03_roulette_releases_possibly_death` - niedeterminizm przez `releases`.
4. `demo_04_roulette_releases_necessary_death_false` - dlaczego przy niedeterminizmie `necessary` może być fałszywe.
5. `demo_05_impossible_load_executable_false` - akcja niewykonalna przez `impossible`.
6. `demo_06_conflict_possibly_p` - akcja złożona, konflikt i `possibly`.
7. `demo_07_conflict_necessary_p_false` - akcja złożona, konflikt i `necessary`.
8. `demo_08_switches_always_noninertial_alarm` - `always`, `noninertial` i konsekwencje stanu.

## Krótkie wyjaśnienie całego mechanizmu

Program działa tak:

1. Czyta domenę, czyli fluenty, akcje i reguły.
2. Generuje legalne stany świata.
3. Generuje stany początkowe zgodne z `initially`.
4. Dla każdej akcji liczy możliwe stany następne.
5. Przy akcjach złożonych wykrywa konflikty i rozważa maksymalne bezkonfliktowe dekompozycje.
6. Dla procesu buduje drzewo możliwych wykonań.
7. Kwerenda `possibly` sprawdza, czy istnieje dobra ścieżka.
8. Kwerenda `necessary` sprawdza, czy wszystkie ścieżki spełniają warunek.

## Najkrótsza gadka do prowadzącej

Reasoner operuje na stanach logicznych. Fluent to zmienna prawda/fałsz, a stan to pełne przypisanie wartości wszystkim fluentom. Domena opisuje, jakie akcje są możliwe i jakie mają efekty. Program rozwija wszystkie możliwe ścieżki wykonania procesu i na końcu sprawdza kwerendę. `Possibly` oznacza, że wystarczy jedna ścieżka spełniająca warunek. `Necessary` oznacza, że warunek musi zachodzić na każdej możliwej ścieżce.
