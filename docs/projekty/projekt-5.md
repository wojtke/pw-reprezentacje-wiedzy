# Projekt 5: Programy działań w środowisku wieloagentowym

Semestr letni 2025/2026 — Reprezentacja Wiedzy

## Klasa systemów dynamicznych DS5

Dana jest klasa DS5 systemów dynamicznych spełniających następujące warunki:

- **Z1.** Prawo inercji.
- **Z2.** Sekwencyjność i niedeterminizm akcji.
- **Z3.** Pełna informacja o wszystkich zmiennych, agentach oraz wszystkich akcjach i ich skutkach bezpośrednich.
- **Z4.** Z każdą akcją związany jest:
  - warunek początkowy i końcowy (reprezentowane przez formuły zdaniowe);
  - zbiór jej wykonawców (agentów).
- **Z5.** Efekt końcowy akcji zależy od stanu, w którym akcja się zaczyna, oraz zbioru jej wykonawców.
- **Z6.** W pewnych stanach akcje mogą być niewykonalne przez wszystkich/wskazanych agentów.
- **Z7.** Dozwolony jest częściowy opis stanów.
- **Z8.** Warunki integralności wpływają tylko na skutki pośrednie akcji.

Programem działań jest ciąg \(\Pi = ((A_1, G_1), \ldots, (A_n, G_n))\), gdzie \(A_i\) jest akcją, zaś \(G_i \subseteq Ag\) jest grupą agentów, gdzie \(Ag\) jest zbiorem wszystkich agentów dziedziny.

Mówimy, że agent \(ag\) jest **aktywny** w wykonaniu akcji *Action* w stanie \(\sigma\), jeśli z opisu dziedziny jednoznacznie wynika jego przynależność do grupy realizującej tę akcję w stanie \(\sigma\). Agent \(ag\) jest aktywny w wykonaniu programu \(\Pi\) działań, jeśli w tym programie występuje akcja \(A_i\), w wykonaniu której jest on aktywny (w stanie wynikającym z wykonania programu).

## Zadanie

Opracować i zaimplementować język akcji dla specyfikacji klasy DS5 systemów dynamicznych oraz odpowiadający mu język kwerend zapewniający uzyskanie odpowiedzi na następujące pytania:

- **Q1.** Czy podany program działań jest możliwy do realizacji zawsze/kiedykolwiek w stanie początkowym?
- **Q2.** Czy wykonanie podanego programu działań ze stanu początkowego prowadzi zawsze/kiedykolwiek do stanu spełniającego warunek celu \(\gamma\)?
- **Q3.** Czy agent \(ag\) jest aktywny w realizacji programu \(\Pi\) działań?

> **Uwaga:** Pod pojęciem *kiedykolwiek* (odpowiednio: *zawsze*) rozumiemy: przy pewnej (odpowiednio: każdej) realizacji programu działań.

## Wskazówki

- W opisie dziedziny grupy agentów można reprezentować przez tzw. **formuły agentowe** analogiczne do formuł zdaniowych. Przykładowo, formuła agentowa \(\Phi_1 = \text{Janek} \wedge \neg\text{Kuba}\) oznacza dowolną grupę agentową zawierającą agenta Janek ORAZ nie zawierającą agenta Kuba, zaś formuła \(\Phi_2 = \text{Janek} \vee \text{Antek}\) oznacza dowolną grupę agentów zawierającą agenta Janek LUB agenta Antek. Jeśli więc \(Ag = \{\text{Janek, Antek, Kuba}\}\) jest zbiorem wszystkich agentów dziedziny, to formule \(\Phi_1\) odpowiadają grupy: \(\{\text{Janek}\}\) i \(\{\text{Janek, Antek}\}\), zaś formule \(\Phi_2\) wszystkie podgrupy \(Ag\) z wyjątkiem \(\emptyset\) oraz \(\{\text{Kuba}\}\).

- Załóżmy, że z opisu dziedziny (zdania typu "causes" i "releases") wynika, że w stanie \(\sigma\) akcję *Action* może wykonać grupa agentów \(\Phi\). Agent \(ag\) jest aktywny w wykonaniu *Action* w \(\sigma\), jeżeli \(\Phi \models ag\). Przykładowo, \(\Phi_1 \models \text{Janek}\), \(\Phi_1 \not\models \text{Antek}\), \(\Phi_2 \not\models ag\) dla dowolnego \(ag \in Ag\). Zatem aktywność \(ag\) w programie \(\Pi\) nie oznacza, że \(ag \in G_i\) dla pewnej pary \((A_i, G_i)\) w tym programie. I tak, niech zdanie `Action by Φ₂ causes γ` należy do dziedziny. Odpowiedzią na kwerendę "czy Janek jest aktywny w programie (Action, {Janek})" jest **NIE**.
