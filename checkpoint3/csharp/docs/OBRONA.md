# Jak to obronić w 60 sekund

Projekt implementuje mały reasoner dla DS4. Wejściem jest opis dziedziny i jedna kwerenda.

Najpierw parser zamienia tekst na AST: formuły, akcje, reguły `causes`, `releases`, `impossible`, `always`, `initially`.

Potem budujemy model:

1. generujemy wszystkie wartościowania fluentów,
2. filtrujemy je przez `always`, otrzymując `Σ`,
3. filtrujemy `Σ` przez `initially`, otrzymując `Σ₀`.

Dla akcji prostej liczymy `Res(A, σ)`:

1. jeśli aktywne jest `impossible`, wynik jest pusty,
2. zbieramy aktywne efekty `causes`,
3. kandydatami są stany z `Σ`, które spełniają efekty,
4. wybieramy stany minimalne względem `New`, czyli względem liczby koniecznych zmian - to realizuje inercję.

Dla akcji złożonej:

1. budujemy graf konfliktu,
2. generujemy maksymalne bezkonfliktowe dekompozycje,
3. liczymy wynik każdej dekompozycji,
4. sumujemy wyniki.

Dla procesu budujemy drzewo wykonań. Kwerendy `possibly` sprawdzają istnienie dobrej ścieżki, a `necessary` wymagają, żeby wszystkie ścieżki były dobre i żadna nie zablokowała się przed końcem procesu.

Najważniejsze zdanie:

> To jest implementacja przez pełną eksplorację przestrzeni stanów i podzbiorów akcji. Nie jest asymptotycznie optymalna, ale dla małych przykładów projektowych jest prosta, poprawna i zgodna z formalną semantyką.
