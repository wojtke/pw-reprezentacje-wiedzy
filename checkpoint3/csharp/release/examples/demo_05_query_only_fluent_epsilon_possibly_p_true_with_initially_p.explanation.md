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
