# Demo 03 - niedeterminizm przez releases i possibly

## Cel przykładu

Ten przykład pokazuje regułę `releases`, czyli zwolnienie fluentu spod zwykłej inercji.

## Domena

```text
fluents loaded, alive
actions spin, shoot, load
initially alive
spin releases loaded if true
shoot causes !alive if loaded
impossible load if loaded
load causes loaded if !loaded
```

Fluent `alive` oznacza, że postać żyje. Fluent `loaded` oznacza, że broń jest załadowana.

Reguła:

```text
spin releases loaded if true
```

mówi, że po akcji `spin` wartość `loaded` może się zmienić niedeterministycznie. Reasoner nie musi zachować poprzedniej wartości `loaded`.

Reguła:

```text
shoot causes !alive if loaded
```

mówi, że strzał zabija tylko wtedy, gdy broń jest załadowana.

## Kwerenda

```text
possibly !alive after spin; shoot
```

Pytamy, czy istnieje ścieżka, w której po wykonaniu `spin`, a potem `shoot`, postać nie żyje.

## Przebieg

Po `spin` fluent `loaded` jest zwolniony. To znaczy, że możliwy jest stan, w którym `loaded` jest prawdziwe, oraz stan, w którym `loaded` jest fałszywe.

Jeśli po `spin` mamy:

```text
loaded = true
```

wtedy `shoot` powoduje:

```text
!alive
```

Jeśli po `spin` mamy:

```text
loaded = false
```

wtedy `shoot` nie zabija.

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Istnieje przynajmniej jedna ścieżka, na której broń jest załadowana po `spin`, a potem `shoot` powoduje `!alive`.
