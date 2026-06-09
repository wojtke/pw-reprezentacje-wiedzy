# Demo 08 - releases daje possibly TAK z każdego stanu początkowego

Oczekiwany wynik: TAK

`initially true` dopuszcza dwa stany początkowe względem `p`:

```text
{¬p}
{p}
```

Akcja:

```text
toss releases p if true
```

uwalnia fluent `p`. To oznacza, że po akcji `p` może przyjąć różne wartości, zamiast być zwyczajnie utrzymany przez inercję.

Z obu stanów początkowych istnieje ścieżka kończąca się z `p`:

```text
{¬p} -> {p}
{p}  -> {p}
```

Dlatego `possibly p after toss` daje TAK.
