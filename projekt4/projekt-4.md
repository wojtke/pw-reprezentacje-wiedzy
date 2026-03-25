# Projekt 4: Procesy działań złożonych

Semestr letni 2025/2026 — Reprezentacja Wiedzy

## Klasa systemów dynamicznych DS4

Dana jest klasa DS4 systemów dynamicznych spełniających następujące warunki:

- **Z1.** Prawo inercji.
- **Z2.** Pełna informacja o wszystkich zmiennych ze zbioru \(F\), wszystkich akcjach ze zbioru \(A\) oraz wszystkich skutkach tych akcji.
- **Z3.** Niedeterminizm działań.
- **Z4.** Z każdą akcją związany jest warunek wstępny oraz warunek wynikowy, oba warunki reprezentowane formułami zdaniowymi; jeśli warunek wstępny nie jest spełniony, wówczas odpowiadający mu wynik akcji jest efektem pustym.
- **Z5.** W pewnych stanach akcja może być niewykonalna.
- **Z6.** Warunki integralności wpływają na skutki uboczne akcji.
- **Z7.** Dopuszczalny jest opis częściowy zarówno stanu początkowego, jak i pewnych stanów wynikających z wykonań sekwencji akcji.

Akcją prostą jest dowolna akcja \(A \in A\). W opisach dziedzin występują jedynie akcje proste. Akcją złożoną \(\mathbb{A}\) jest zbiór \(\mathbb{A}\) akcji prostych, czyli \(\mathbb{A} \subseteq A\). Takie akcje występują w języku kwerend. Akcja złożona \(\mathbb{A} = \{A_1, \ldots, A_k\}\) jest wykonalna w stanie \(\sigma\) jeśli:

- każde dwie akcje składowe \(A_i, A_j \in \mathbb{A}\) (\(i \neq j\)) wpływają w stanie \(\sigma\) na rozłączne zbiory zmiennych;
- wszystkie akcje składowe \(A \in \mathbb{A}\) są wykonalne w stanie \(\sigma\).

Procesem nazywamy ciąg \(P = (\mathbb{A}_1, \ldots, \mathbb{A}_n)\) akcji złożonych, \(n > 0\).

## Zadanie

Opracować i zaimplementować język akcji dla specyfikacji podanej klasy DS4 systemów dynamicznych oraz odpowiadający mu język kwerend zapewniający uzyskanie odpowiedzi na następujące zapytania:

- **Q1.** Czy podany proces \(P\) jest wykonalny zawsze/kiedykolwiek ze stanu początkowego systemu?
- **Q2.** Czy podany proces \(P\) prowadzi zawsze/kiedykolwiek do osiągnięcia celu \(\gamma\) ze stanu początkowego?
