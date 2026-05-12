# Demo 01 - warunkowy efekt i possibly

## Cel przykładu

Ten przykład pokazuje, jak nieokreślony fluent w stanie początkowym wpływa na kwerendę `possibly` przy semantyce ∀σ ∃ścieżka.

Reguła:

```text
action causes q if p
```

Akcja `action` powoduje `q`, ale tylko wtedy, gdy przed wykonaniem akcji prawdziwe jest `p`.

## Domena

```text
fluents p, q
actions action
initially !q
action causes q if p
```

Warunek początkowy mówi tylko, że `q` jest fałszywe. Nie mówi nic o `p`, więc reasoner rozważa oba możliwe stany początkowe:

```text
{¬p, ¬q}
{p, ¬q}
```

## Kwerenda

```text
possibly q after action
```

Pytamy, czy z kadego stanu poczatkowego istnieje jakakolwiek ścieżka wykonania, po której po akcji `action` fluent `q` będzie prawdziwy.

## Przebieg

Gałąź pierwsza – stan początkowy `{¬p, ¬q}`. Warunek `p` nie zachodzi. Akcja nie wymusza `q`, więc przez inercję zostaje:

```text
{¬p, ¬q} -> {¬p, ¬q}
```

W tej gałęzi nie istnieje żadna ścieżka prowadząca do stanu z `q`.

Gałąź druga – stan początkowy `{p, ¬q}`. Warunek `p` zachodzi. Akcja wymusza `q`:

```text
{p, ¬q} -> {p, q}
```

## Odpowiedź

Odpowiedź:

```text
NIE
```

Choć w drugiej gałęzi `q` da się osiągnąć, w pierwszej gałęzi (`¬p`) żadna ścieżka nie prowadzi do `q`. Kwerenda `possibly` wymaga istnienia takiej ścieżki dla **każdego** stanu początkowego, więc cała kwerenda jest fałszywa.

Aby otrzymać `TAK`, należałoby zawęzić stan początkowy, np.

```text
initially !q and p
```

– wtedy jedyny stan początkowy to `{p, ¬q}`, w którym akcja wymusza `q` i kwerenda jest spełniona.
