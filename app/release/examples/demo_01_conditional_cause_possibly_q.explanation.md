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
initially p and !q
action causes q if p
```

Mamy dwa fluenty: `p` i `q`. Warunek początkowy mówi, że `p` jest prawdziwe, a `q` fałszywe, więc jest dokładnie jeden stan początkowy:

```text
{p, ¬q}
```

## Kwerenda

```text
possibly q after action
```

Kwerenda `possibly` jest prawdziwa, gdy z każdego stanu początkowego istnieje przynajmniej jedna pełna ścieżka kończąca się stanem spełniającym cel.

## Przebieg

W jedynym stanie początkowym `{p, ¬q}` warunek `p` zachodzi. Akcja wymusza `q`, więc wynik to:

```text
{p, ¬q} -> {p, q}
```

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Z jedynego stanu początkowego istnieje ścieżka prowadząca do stanu z `q`.

## Uwaga o opisie częściowym

Gdyby warunek początkowy mówił tylko `initially !q` (nic o `p`), stanami początkowymi byłyby `{p, ¬q}` i `{¬p, ¬q}`. Ze stanu `{¬p, ¬q}` żadna ścieżka nie prowadzi do `q`, więc przy semantyce "dla każdego stanu początkowego istnieje ścieżka" odpowiedź brzmiałaby NIE.
