# Demo 01 - action jako nazwa akcji, possibly q daje NIE

Oczekiwany wynik: NIE

Ten przykład jest najważniejszy po poprawce semantyki `possibly`.

Nie deklarujemy osobno:

```text
fluents p, q
actions action
```

Parser powinien sam wywnioskować:

```text
p, q    - fluenty
action  - akcja
```

Stan początkowy mówi tylko `!q`, więc `p` pozostaje nieokreślone. Są więc dwa stany początkowe:

```text
{¬p, ¬q}
{p, ¬q}
```

Reguła:

```text
action causes q if p
```

działa tylko w gałęzi, gdzie `p` jest prawdziwe.

Trace logicznie powinien zawierać:

```text
{¬p, ¬q} -> {¬p, ¬q}
{p, ¬q}  -> {p, q}
```

Po poprawce `possibly` nie znaczy już tylko "istnieje jedna dobra gałąź globalnie". Znaczy:

```text
dla każdego stanu początkowego istnieje pełna ścieżka spełniająca cel
```

Z początkowego `{¬p, ¬q}` nie da się dojść do `q`, więc wynik to NIE.
