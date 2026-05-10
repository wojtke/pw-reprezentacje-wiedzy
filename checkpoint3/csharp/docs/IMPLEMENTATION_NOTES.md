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

- `possibly` wymaga istnienia jednej dobrej pełnej ścieżki,
- `necessary` wymaga, żeby wszystkie ścieżki były dobre i żadna nie blokowała się przed końcem procesu.
