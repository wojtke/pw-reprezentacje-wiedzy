# Demo 01 - warunkowy efekt i possibly

## Cel przykładu

Ten przykład pokazuje działanie reguły:

```text
action causes q if p
```

Czyli akcja `action` powoduje `q`, ale tylko wtedy, gdy przed wykonaniem akcji prawdziwe jest `p`.

## Domena

```text
fluents p, q
actions action
initially !q
action causes q if p
```

Mamy dwa fluenty: `p` i `q`. Warunek początkowy mówi tylko, że `q` jest fałszywe. Nie mówi nic o `p`, więc reasoner musi rozważyć oba przypadki:

```text
{¬p, ¬q}
{p, ¬q}
```

## Kwerenda

```text
possibly q after action
```

Pytamy, czy istnieje jakakolwiek ścieżka wykonania, po której po akcji `action` fluent `q` będzie prawdziwy.

## Przebieg

Dla stanu początkowego `{¬p, ¬q}` warunek `p` nie zachodzi. Akcja nie wymusza `q`, więc przez inercję zostaje:

```text
{¬p, ¬q} -> {¬p, ¬q}
```

Dla stanu początkowego `{p, ¬q}` warunek `p` zachodzi. Akcja wymusza `q`, więc wynik to:

```text
{p, ¬q} -> {p, q}
```

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Istnieje ścieżka prowadząca do stanu z `q`, więc kwerenda `possibly q after action` jest prawdziwa.
