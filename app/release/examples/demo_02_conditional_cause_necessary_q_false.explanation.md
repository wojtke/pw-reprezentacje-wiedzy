# Demo 02 - warunkowy efekt i necessary

## Cel przykładu

Ten przykład pokazuje różnicę między `possibly` i `necessary`.

Domena jest taka sama jak w Demo 01:

```text
fluents p, q
actions action
initially !q
action causes q if p
```

## Stany początkowe

Warunek `initially !q` ustala tylko wartość `q`. Fluent `p` nie jest określony, więc możliwe są dwa stany początkowe:

```text
{¬p, ¬q}
{p, ¬q}
```

## Kwerenda

```text
necessary q after action
```

Pytamy, czy po wykonaniu akcji `action` na każdej możliwej ścieżce `q` będzie prawdziwe.

## Przebieg

Gałąź pierwsza:

```text
{¬p, ¬q} -> {¬p, ¬q}
```

Tutaj warunek `p` nie zachodzi, więc `q` nie zostaje ustawione.

Gałąź druga:

```text
{p, ¬q} -> {p, q}
```

Tutaj warunek `p` zachodzi, więc akcja ustawia `q`.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

`q` jest prawdziwe tylko w jednej z możliwych gałęzi. Kwerenda `necessary` wymaga, żeby warunek był spełniony we wszystkich gałęziach, więc wynik jest fałszywy.
