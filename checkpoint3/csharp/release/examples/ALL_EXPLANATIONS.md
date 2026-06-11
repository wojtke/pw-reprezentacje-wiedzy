# Zbiorcze wyjaśnienia przykładów

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


---

# Demo 02 - action jako nazwa akcji, possibly q daje TAK przy initially p

Oczekiwany wynik: TAK

To jest pozytywna wersja poprzedniego przykładu.

Nadal nie deklarujemy osobno fluentów ani akcji. Parser wyciąga je z kontekstu.

Stan początkowy jest teraz jednoznaczny:

```text
initially p and !q
```

Czyli jedyny stan początkowy to:

```text
{p, ¬q}
```

Reguła:

```text
action causes q if p
```

ma spełniony warunek, więc po akcji dostajemy:

```text
{p, ¬q} -> {p, q}
```

Dla każdego stanu początkowego istnieje ścieżka kończąca się z `q`. Jest tylko jeden stan początkowy, więc `possibly q after action` daje TAK.


---

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


---

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


---

# Demo 05 - possibly p after epsilon daje TAK, gdy initially p

Oczekiwany wynik: TAK

To jest pozytywna wersja poprzedniego przykładu.

Domena mówi:

```text
initially p
```

Jest więc tylko jeden dopuszczalny stan początkowy względem fluentu `p`:

```text
{p}
```

Proces jest pusty:

```text
epsilon
```

Stan końcowy nadal spełnia `p`, więc `possibly p after epsilon` daje TAK.

Ten przykład dobrze pokazuje, że `possibly` z pustym procesem zachowuje się jak sprawdzenie celu dla wszystkich stanów początkowych.


---

# Demo 06 - akcja z kwerendy bez reguł jest no-op

Oczekiwany wynik: TAK

Akcja `do_nothing` nie jest zadeklarowana w domenie i nie ma żadnych reguł.

Pojawia się jednak w kwerendzie:

```text
possibly p after do_nothing
```

Program powinien dodać ją jako akcję z kontekstu procesu.

Ponieważ nie ma dla niej reguł `causes`, `releases` ani `impossible`, działa jak akcja pusta. Inercja zachowuje `p`.

```text
{p} -> {p}
```

Dlatego wynik to TAK.


---

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


---

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


---

# Demo 09 - fluent jako nazwa akcji

Oczekiwany wynik: TAK

Ten przykład potwierdza, że nazwa akcji może wyglądać jak słowo kluczowe.

Akcja nazywa się:

```text
fluent
```

Linia:

```text
fluent causes q if p
```

ma zostać rozpoznana jako reguła akcji, a nie jako deklaracja fluentów.

Stan początkowy:

```text
{p, ¬q}
```

spełnia warunek `p`, więc po akcji dostajemy:

```text
{p, q}
```

Wynik `possibly q after fluent` to TAK.


---

# Demo 10 - mieszanka deklaracji jawnych i symboli z kontekstu

Oczekiwany wynik: TAK

Ten przykład pokazuje, że deklaracje jawne i automatyczne wykrywanie symboli mogą działać razem.

Jawnie deklarujemy tylko:

```text
fluents p
```

Ale w regule pojawia się `r` oraz akcja `setR`:

```text
setR causes r if p
```

Program powinien dodać je z kontekstu.

Stan początkowy spełnia `p`, więc akcja ustawia `r`.

```text
{p, ¬r} -> {p, r}
```

Wynik `possibly r after setR` to TAK.


---
