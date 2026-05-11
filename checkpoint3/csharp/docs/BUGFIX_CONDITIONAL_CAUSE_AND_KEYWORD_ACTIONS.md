# Poprawka: akcja o nazwie `action` i warunkowy `causes`

Problem:

```text
fluents p, q
actions action
initially !q
action causes q if p
```

Parser traktowal linie:

```text
action causes q if p
```

jak deklaracje akcji, poniewaz zaczynala sie od slowa `action`.
W efekcie nie powstawala regula `CauseRule`, wiec akcja nie wymuszala `q` w galezi, gdzie `p` bylo prawdziwe.

Poprawka:

- w `DomainParser.ParseLine` reguly `causes` i `releases` sa sprawdzane przed deklaracjami `action/actions` oraz `fluent/fluents`,
- dodano testy regresyjne dla akcji nazwanej `action`,
- dodano testy regresyjne dla akcji nazwanej `fluent`,
- istniejace testy `ConditionalCauseRegressionTests` powinny teraz przechodzic.

Oczekiwany trace:

```text
[0] start -> {p, ¬q}
  [1] action -> {p, q}
```

Kwerenda:

```text
possibly q after action
```

powinna zwracac `TAK`.
