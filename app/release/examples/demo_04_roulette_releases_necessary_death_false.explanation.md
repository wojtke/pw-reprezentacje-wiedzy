# Demo 04 - releases i necessary

## Cel przykładu

Ten przykład pokazuje, że `possibly` i `necessary` mogą dawać różne odpowiedzi w domenach niedeterministycznych.

## Domena

Domena jest taka sama jak w Demo 03:

```text
fluents loaded, alive
actions spin, shoot, load
initially alive
spin releases loaded if true
shoot causes !alive if loaded
impossible load if loaded
load causes loaded if !loaded
```

## Kwerenda

```text
necessary !alive after spin; shoot
```

Pytamy, czy po wykonaniu `spin`, a potem `shoot`, postać będzie martwa na każdej możliwej ścieżce.

## Przebieg

Po `spin` fluent `loaded` jest zwolniony. To daje co najmniej dwie intuicyjne możliwości:

```text
loaded = true
loaded = false
```

Jeśli `loaded = true`, wtedy `shoot` powoduje `!alive`.

Jeśli `loaded = false`, wtedy `shoot` nie ma aktywnego efektu zabicia, więc `alive` zostaje zachowane przez inercję.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Nie wszystkie ścieżki prowadzą do `!alive`. Wystarczy jedna ścieżka, na której broń nie jest załadowana i postać przeżywa, żeby `necessary !alive` było fałszywe.
