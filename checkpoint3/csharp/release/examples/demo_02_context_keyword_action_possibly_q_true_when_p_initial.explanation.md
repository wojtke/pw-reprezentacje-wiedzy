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
