# Projekt 1: Scenariusze działań

Semestr letni 2025/2026 — Reprezentacja Wiedzy

## Klasa systemów dynamicznych DS1

Dana jest klasa DS1 systemów dynamicznych spełniających następujące warunki:

- **Z1.** Prawo inercji.
- **Z2.** Sekwencyjność i niedeterminizm działań.
- **Z3.** Pełna informacja o wszystkich akcjach i wszystkich ich skutkach bezpośrednich.
- **Z4.** Liniowy model czasu (czas dyskretny).
- **Z5.** Z każdą akcją związany jest:
  - Warunek początkowy (ew. `true`);
  - Efekt (cząstkowy, końcowy) akcji, dopuszczalny jest opis przebiegu akcji (tzn. efekty występują w różnych momentach trwania akcji);
  - Czas *d > 1*, po którym występuje efekt akcji; w trakcie wykonywania akcji może wystąpić kilka jej efektów, z których każdy może wystąpić w różnych punktach czasowych realizacji tej akcji.
- **Z6.** Wartości wszystkich zmiennych, na które akcja ma/może mieć wpływ w pewnym przedziale czasowym jej realizacji, są nieznane.
- **Z7.** Akcja może mieć skutki środowiskowe (zmiany wartości zmiennych systemu) lub dynamiczne (wystąpienie innych akcji po *t > 0* jednostkach czasu od zakończenia akcji przyczynowej).
- **Z8.** W pewnych stanach akcje mogą być niewykonalne. Stany takie podane są albo przez podanie konkretnych punktów czasowych, albo przez określenie warunków logicznych.
- **Z9.** Pewne stany mogą powodować wystąpienie akcji.

Pojęcie scenariusza rozumiemy w sensie języka AL. Scenariusz jest realizowalny, jeżeli wszystkie przewidziane w nim akcje (bezpośrednio i pośrednio) są wykonalne — czyli istnieje model scenariusza względem opisu dziedziny.

## Zadanie

Opracować i zaimplementować język akcji dla specyfikacji podanej klasy systemów dynamicznych oraz odpowiadający mu język kwerend zapewniający uzyskanie odpowiedzi na następujące pytania:

- **Q1.** Czy podany scenariusz jest możliwy do realizacji?
- **Q2.** Czy w chwili *t* realizacji scenariusza wykonywana jest akcja *A*?
- **Q3.** Czy w chwili *t > 0* realizacji podanego scenariusza warunek *γ* zachodzi zawsze/kiedykolwiek?
