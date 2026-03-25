# Projekt 2: Programy działań z efektami domyślnymi

Semestr letni 2025/2026 — Reprezentacja Wiedzy

## Klasa systemów dynamicznych DS2

Dana jest klasa DS2 systemów dynamicznych spełniających następujące warunki:

- **Z1.** Prawo inercji.
- **Z2.** Niedeterminizm i sekwencyjność działań.
- **Z3.** Pełna informacja o wszystkich akcjach, agentach i zmiennych.
- **Z4.** Z każdą akcją związany jest:
  - warunek początkowy (ew. `true`);
  - efekt akcji;
  - jej wykonawca.
- **Z5.** Skutki akcji:
  - pewne (zawsze występują po zakończeniu akcji);
  - domyślne (preferowane, zachodzą po zakończeniu akcji, o ile nie jest wiadomym, że nie występują).
- **Z6.** Efekty akcji zależą od jej stanu, w którym akcja się zaczyna, oraz wykonawcy tej akcji.
- **Z7.** W pewnych stanach akcje mogą być niewykonalne przez pewnych (wszystkich) wykonawców.

Programem działań nazywamy ciąg \(((A_1, w_1), (A_2, w_2), \ldots, (A_n, w_n))\), gdzie \(A_i\) jest akcją, zaś \(w_i\) jej wykonawcą (agentem) lub \(\varepsilon\) (ktokolwiek).

## Zadanie

Opracować i zaimplementować język akcji dla specyfikacji podanej klasy DS2 systemów dynamicznych oraz odpowiadający mu język kwerend zapewniający uzyskanie odpowiedzi na następujące pytania:

- **Q1.** Czy podany program działań jest wykonywalny zawsze/kiedykolwiek?
- **Q2.** Czy wykonanie podanego programu działań z dowolnego stanu spełniającego warunek \(\pi\) prowadzi zawsze/kiedykolwiek/na ogół do stanu spełniającego warunek celu \(\gamma\)?
- **Q3.** Czy wskazany wykonawca jest zaangażowany w realizację programu zawsze/kiedykolwiek?
