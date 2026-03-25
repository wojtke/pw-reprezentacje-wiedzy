# Projekt 3: Realizacja programów działań

Semestr letni 2025/2026 — Reprezentacja Wiedzy

## Klasa systemów dynamicznych DS3

Dana jest klasa DS3 systemów dynamicznych spełniających następujące warunki:

- **Z1.** Prawo inercji.
- **Z2.** Pełna informacja o wszystkich akcjach i wszystkich zmiennych.
- **Z3.** Sekwencyjność i niedeterminizm działań.
- **Z4.** Z każdą akcją związany jest:
  - warunek wstępny;
  - warunek wynikowy;
  - koszt realizacji (\(k \in \mathbb{N}\)).
- **Z5.** Skutki akcji zależą od:
  - stanu początkowego jej wykonania;
  - kosztu jej realizacji.
- **Z6.** W pewnych stanach akcja może być niewykonalna.
- **Z7.** Jeśli wykonanie akcji w stanie \(\sigma\) prowadzi do \(n\) efektów o kosztach odpowiednio \(k_1, \ldots, k_n\), to koszt całej akcji jest równy \(\max\{k_1, \ldots, k_n\}\).
- **Z8.** Warunki integralności wpływają na skutki uboczne akcji.
- **Z9.** Dopuszczalny jest opis częściowy zarówno stanu początkowego, jak i pewnych stanów wynikających z wykonań sekwencji akcji.

Programem działań nazywamy ciąg \(P = (A_1, \ldots, A_n)\) akcji, \(n > 0\).

## Zadanie

Opracować i zaimplementować język akcji dla specyfikacji podanej klasy DS3 systemów dynamicznych oraz odpowiadający mu język kwerend zapewniający uzyskanie odpowiedzi na następujące zapytania:

- **Q1.** Czy podany program działań jest realizowalny zawsze/kiedykolwiek z każdego stanu spełniającego warunek \(\pi\) przy podanym koszcie \(\kappa\)?
- **Q2.** Czy podany program działań prowadzi zawsze/kiedykolwiek do osiągnięcia celu \(\gamma\) ze stanu:
  - początkowego;
  - spełniającego warunek \(\pi\)?
- **Q3.** Przy jakim koszcie cel \(\gamma\) jest zawsze/kiedykolwiek osiągalny ze stanu początkowego?
