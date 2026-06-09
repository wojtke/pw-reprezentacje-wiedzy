# Demo 07 - konflikt akcji złożonych i necessary

## Cel przykładu

Ten przykład używa tej samej domeny co Demo 06, ale pokazuje odpowiedź dla kwerendy `necessary`.

## Domena

```text
fluents p
actions make_p, make_not_p
initially !p
make_p causes p if true
make_not_p causes !p if true
```

## Kwerenda

```text
necessary p after {make_p,make_not_p}
```

Pytamy, czy po wykonaniu kroku złożonego `{make_p,make_not_p}` fluent `p` jest prawdziwy na każdej możliwej ścieżce.

## Dekompozycje

Ponieważ akcje są w konflikcie, reasoner rozważa maksymalne bezkonfliktowe dekompozycje:

```text
{make_p}
{make_not_p}
```

Pierwsza prowadzi do:

```text
{p}
```

Druga prowadzi do:

```text
{¬p}
```

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Nie każda możliwa dekompozycja prowadzi do `p`. Kwerenda `necessary p` jest więc fałszywa.
