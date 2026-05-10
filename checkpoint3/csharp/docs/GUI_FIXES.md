# Poprawki GUI i pamięci

W tej wersji poprawiono dwa problemy zauważone przy klikaniu `Oblicz`:

1. **Wzrost pamięci po kolejnych kliknięciach**
   - wynik jest teraz czyszczony przez pomocniczą funkcję `SetTextNoUndo`,
   - po każdej zmianie tekstu czyszczony jest bufor cofania `TextBox.ClearUndo()`,
   - renderowany trace jest limitowany do ok. 60 000 znaków,
   - po obliczeniu aplikacja prosi GC o sprzątnięcie dużych tymczasowych struktur.

2. **Czytelność okna**
   - większe fonty,
   - domyślne zawijanie tekstu,
   - większe okno startowe,
   - czytelniejszy układ przycisków i pól tekstowych,
   - osobny przycisk `Wyczyść wynik`.

Uwaga: niewielki wzrost pamięci po pierwszym uruchomieniu jest normalny dla .NET/WinForms, bo runtime ładuje biblioteki i JIT-uje kod. Ważne jest, żeby pamięć nie rosła liniowo bez końca po każdym kolejnym kliknięciu.
