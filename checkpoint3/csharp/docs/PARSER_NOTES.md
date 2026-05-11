# Uwagi o parserze

Parser dziedziny jest ostrożnie dopasowany do notacji z wykładów. Nie trzeba deklarować osobnych sekcji `fluents` ani `actions`.

Fluenty są zbierane z formuł występujących w zdaniach:

```text
initially !q
action causes q if p
```

W tym przykładzie parser sam wykrywa fluenty `p` i `q` oraz akcję `action`. Nazwa `action` może być użyta jako normalna nazwa akcji, ponieważ opcjonalne deklaracje `actions ...` są rozpoznawane dopiero po sprawdzeniu zdań `causes`, `releases` i `impossible`.

Obsługiwane są też średniki na końcu zdań, więc przykłady wykładowe typu:

```text
initially !loaded and alive;
Load causes loaded;
Shoot causes !loaded;
Shoot causes !alive if loaded;
```

są przyjmowane bez dodatkowych deklaracji.

W języku procesu średnik nadal oznacza kolejność kroków, np. `Load; Shoot`.
