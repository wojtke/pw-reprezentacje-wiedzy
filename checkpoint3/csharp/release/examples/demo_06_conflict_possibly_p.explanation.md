# Demo 06 - konflikt akcji złożonych i possibly

## Cel przykładu

Ten przykład pokazuje akcję złożoną oraz konflikt między akcjami.

## Domena

```text
fluents p
actions make_p, make_not_p
initially !p
make_p causes p if true
make_not_p causes !p if true
```

Akcja `make_p` próbuje ustawić `p` na prawdę.

Akcja `make_not_p` próbuje ustawić `p` na fałsz.

## Kwerenda

```text
possibly p after {make_p,make_not_p}
```

Pytamy, czy po wykonaniu złożonego kroku `{make_p,make_not_p}` możliwe jest, że `p` będzie prawdziwe.

## Konflikt

Krok:

```text
{make_p,make_not_p}
```

oznacza próbę równoległego wykonania obu akcji.

Akcje są jednak w konflikcie, bo jedna wymusza:

```text
p
```

a druga wymusza:

```text
!p
```

Reasoner nie wykonuje sprzecznych efektów naraz. Zamiast tego rozważa maksymalne bezkonfliktowe dekompozycje, czyli tutaj:

```text
{make_p}
{make_not_p}
```

## Przebieg

Po dekompozycji `{make_p}` dostajemy:

```text
{p}
```

Po dekompozycji `{make_not_p}` dostajemy:

```text
{¬p}
```

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Istnieje dekompozycja prowadząca do stanu z `p`, więc kwerenda `possibly p` jest prawdziwa.
