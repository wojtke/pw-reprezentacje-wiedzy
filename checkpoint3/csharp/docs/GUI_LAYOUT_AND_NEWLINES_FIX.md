# GUI layout + newline fix

Zmiany w tej wersji:

- Główna część okna używa teraz `TableLayoutPanel` z dwoma kolumnami 50/50:
  - lewa kolumna: Dziedzina,
  - prawa kolumna: Kwerenda + Wynik.
- Prawa kolumna ma dwa wiersze 50/50, więc Kwerenda i Wynik mają tę samą wysokość.
- Pola tekstowe domeny i kwerendy mają `AcceptsReturn = true`.
- Tekst wstawiany do pól jest normalizowany do `Environment.NewLine`, żeby przykłady z raw stringów C# nie wyświetlały się w WinForms jako jedna długa linia.

Plik zmieniony:

- `src/Ds4.Gui/MainForm.cs`
