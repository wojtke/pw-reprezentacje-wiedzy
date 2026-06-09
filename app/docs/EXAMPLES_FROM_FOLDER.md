# Przykłady z folderu `examples`

GUI nie korzysta już tylko z listy wpisanej na sztywno w kodzie. Przy starcie aplikacja szuka folderu `examples` i ładuje wszystkie pary plików:

```text
<id>.domain
<id>.query
```

Plik `.domain` zawiera opis dziedziny. Może zawierać komentarze zaczynające się od `#`. Dwa komentarze są traktowane specjalnie przez listę w GUI:

```text
# name: Nazwa wyświetlana w GUI
# description: Krótki opis przypadku
```

Plik `.query` zawiera jedną albo kilka kwerend, po jednej na linię. Puste linie i komentarze są ignorowane.

W obecnej paczce są trzy przypadki z odpowiedzią TAK i trzy z odpowiedzią NIE:

```text
tak_01_switches_necessary_alarm        -> TAK
tak_02_roulette_possibly_death         -> TAK
tak_03_unlock_necessary_open           -> TAK
nie_01_roulette_necessary_death        -> NIE
nie_02_impossible_load_executable      -> NIE
nie_03_conflict_necessary_p            -> NIE
```

Jeżeli folder `examples` nie zostanie znaleziony, core ma awaryjną listę wbudowanych przykładów, żeby GUI nadal dało się uruchomić.
