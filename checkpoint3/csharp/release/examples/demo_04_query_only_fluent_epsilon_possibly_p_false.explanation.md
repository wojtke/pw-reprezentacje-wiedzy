# Demo 04 - fluent tylko z kwerendy, possibly p after epsilon daje NIE

Oczekiwany wynik: NIE

Tutaj `p` nie występuje w domenie, tylko w kwerendzie.

Program powinien dodać `p` jako fluent z kontekstu kwerendy.

Domena:

```text
initially true
```

nie narzuca wartości `p`, więc są dwa stany początkowe:

```text
{¬p}
{p}
```

Proces `epsilon` jest pusty, więc stan się nie zmienia.

Po poprawce `possibly p after epsilon` wymaga, aby dla każdego stanu początkowego istniała ścieżka kończąca się z `p`. Dla stanu `{¬p}` nie ma żadnej akcji, która mogłaby zmienić `p`.

Dlatego wynik to NIE.
