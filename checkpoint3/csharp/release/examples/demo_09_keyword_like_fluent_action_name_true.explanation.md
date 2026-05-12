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
