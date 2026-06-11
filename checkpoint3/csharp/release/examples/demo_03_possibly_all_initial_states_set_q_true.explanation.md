# Demo 03 - possibly true, bo akcja ustawia q z każdego stanu początkowego

Oczekiwany wynik: TAK

Ten przykład pokazuje poprawne pozytywne `possibly` przy wielu stanach początkowych.

`initially true` nie narzuca wartości `q`, więc są dwa stany początkowe:

```text
{¬q}
{q}
```

Akcja:

```text
setQ causes q if true
```

ustawia `q` niezależnie od stanu początkowego.

Z każdego stanu początkowego istnieje pełna ścieżka kończąca się z `q`:

```text
{¬q} -> {q}
{q}  -> {q}
```

Dlatego `possibly q after setQ` daje TAK.
