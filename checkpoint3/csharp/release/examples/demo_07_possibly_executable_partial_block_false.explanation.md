# Demo 07 - possibly executable daje NIE, gdy jakiś stan początkowy blokuje proces

Oczekiwany wynik: NIE

To jest przykład na `possibly executable` po poprawce semantyki.

`initially true` zostawia `p` nieokreślone, więc są gałęzie z `p` i z `¬p`.

Reguła:

```text
impossible a if p
```

blokuje akcję `a` w stanach, gdzie `p` jest prawdziwe.

Nawet jeśli z gałęzi `¬p` proces da się wykonać, to z gałęzi `p` nie da się go wykonać do końca.

Po poprawce `possibly executable` wymaga pełnej ścieżki z każdego stanu początkowego, więc wynik to NIE.
