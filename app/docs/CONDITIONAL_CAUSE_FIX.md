# Poprawka conditional cause

Naprawiany przypadek regresyjny, bez deklaracji fluents/actions:

```text
initially !q
action causes q if p
```

Kwerenda:

```text
possibly q after action
```

Poprawny wynik to `TAK`.

Ponieważ `initially !q` nie ustala wartości `p`, istnieją dwa stany początkowe:

```text
{not p, not q}
{p, not q}
```

Dla gałęzi `{p, not q}` warunek `if p` jest spełniony, więc następnik musi mieć `q = true`:

```text
{p, not q} -> {p, q}
```

Dodałem testy regresyjne w:

```text
tests/Ds4.Tests/ConditionalCauseRegressionTests.cs
```

oraz przykłady:

```text
examples/tak_bugfix_01_conditional_cause_possible_q.*
examples/nie_bugfix_01_conditional_cause_not_necessary_q.*
```
