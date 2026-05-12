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
