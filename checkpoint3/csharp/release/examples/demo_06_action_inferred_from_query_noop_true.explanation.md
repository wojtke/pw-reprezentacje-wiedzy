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
