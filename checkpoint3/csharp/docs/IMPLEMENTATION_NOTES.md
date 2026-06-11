# Notatki implementacyjne

## Dlaczego C#?

C# dobrze pasuje do tego projektu, bo pozwala zrobić zarówno czysty core, jak i prostą aplikację desktopową dla Windows. Kod jest statycznie typowany i łatwy do obronienia na zajęciach.

## Dlaczego brute force?

Semantyka DS4 naturalnie prowadzi do pełnej eksploracji:

1. generujemy wszystkie wartościowania fluentów,
2. filtrujemy je przez `always`,
3. filtrujemy stany początkowe przez `initially`,
4. dla akcji sprawdzamy kandydatów w `Σ`,
5. dla akcji złożonych enumerujemy podzbiory akcji.

To jest wykładnicze w najgorszym przypadku, ale przykłady projektowe mają mało fluentów i mało akcji.

## Różnica między possibly i necessary

Obie kwerendy kwantyfikują uniwersalnie po stanach początkowych z `Σ₀` (Z7 dopuszcza opis częściowy, więc `Σ₀` może mieć wiele elementów). Różni je tylko kwantyfikator nad ścieżkami:

- `possibly` wymaga, żeby **dla każdego** `σ₀ ∈ Σ₀` istniała co najmniej jedna pełna dobra ścieżka,
- `necessary` wymaga, żeby **dla każdego** `σ₀ ∈ Σ₀` **wszystkie** ścieżki były dobre i żadna nie blokowała się przed końcem procesu.

W szczególności dla pustego procesu obie kwerendy zachowują się tak samo: warunek musi zachodzić w każdym stanie początkowym.
