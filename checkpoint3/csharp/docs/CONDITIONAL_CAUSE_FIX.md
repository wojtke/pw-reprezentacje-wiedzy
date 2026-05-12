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

Ponieważ `initially !q` nie ustala wartości `p`, istnieją dwa stany początkowe:

```text
{¬p, ¬q}
{p, ¬q}
```

Dla gałęzi `{p, ¬q}` warunek `if p` jest spełniony, więc następnik musi mieć `q = true`:

```text
{p, ¬q} -> {p, q}
```

Na gałęzi `{¬p, ¬q}` warunek `if p` nie zachodzi i akcja jest no-opem — następnik to `{¬p, ¬q}`.

Pod poprawną semantyką kwerend `possibly` = (dla każdego `σ₀ ∈ Σ₀` istnieje ścieżka spełniająca warunek) odpowiedź na `possibly q after action` to **NIE**, bo gałąź `{¬p, ¬q}` nie produkuje `q`. Test regresyjny i przykład w `examples/` zachowują się więc tak:

- `Conditional_Cause_Forces_Effect_On_Branch_Where_Condition_Holds` weryfikuje, że na gałęzi `{p, ¬q}` powstaje `{p, q}` (właściwa naprawa parsera/silnika).
- `Conditional_Cause_Is_Noop_On_Branch_Where_Condition_Does_Not_Hold` weryfikuje, że gałąź `{¬p, ¬q}` nie zmienia stanu.
- `Query_Possibly_Q_After_Action_Is_False_Under_Partial_Initial` weryfikuje, że pełna kwerenda `possibly q after action` daje **NIE**, bo nie każdy stan początkowy ma ścieżkę kończącą się `q`.
- `Query_Necessary_Q_After_Action_Is_False_For_Regression_Domain` weryfikuje, że `necessary q after action` to też **NIE**.

Testy regresyjne znajdują się w:

```text
tests/Ds4.Tests/ConditionalCauseRegressionTests.cs
```

oraz przykłady:

```text
examples/nie_bugfix_01_conditional_cause_possible_q_partial_p.*
examples/nie_bugfix_01_conditional_cause_not_necessary_q.*
```
