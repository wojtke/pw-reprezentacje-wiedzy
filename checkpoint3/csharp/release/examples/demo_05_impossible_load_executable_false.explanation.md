# Demo 05 - impossible i executable

## Cel przykładu

Ten przykład pokazuje różnicę między akcją, która nic nie zmienia, a akcją, której nie wolno wykonać.

## Domena

```text
fluents loaded
actions load
initially loaded
impossible load if loaded
load causes loaded if !loaded
```

Stan początkowy wymusza:

```text
loaded = true
```

Reguła:

```text
impossible load if loaded
```

mówi, że akcji `load` nie można wykonać, jeśli `loaded` jest już prawdziwe.

## Kwerenda

```text
possibly executable after load
```

Pytamy, czy istnieje jakakolwiek ścieżka, na której proces składający się z akcji `load` jest wykonywalny.

## Przebieg

Jedyny stan początkowy to:

```text
{loaded}
```

W tym stanie zachodzi warunek `loaded`, więc reguła `impossible load if loaded` blokuje akcję `load`.

Nie ma żadnego stanu następnego.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Proces nie jest możliwy do wykonania, ponieważ akcja `load` jest zablokowana już w stanie początkowym.
