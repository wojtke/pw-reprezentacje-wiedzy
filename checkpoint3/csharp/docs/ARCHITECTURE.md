# Architektura

Projekt jest podzielony na trzy warstwy.

## 1. Core

`Ds4.Core` zawiera całą logikę niezależną od UI:

- `Model` - fluenty, akcje, stan, dziedzina i reguły,
- `Formula` - AST formuł i konwersja do DNF,
- `Parser` - ręczny parser recursive descent,
- `Semantics` - generacja `Σ`, `Σ₀`, funkcje `Res`, konflikty, dekompozycje, procesy i kwerendy,
- `Api` - facade `Ds4Facade`, przez który rozmawia GUI/CLI.

## 2. CLI

`Ds4.Cli` pozwala szybko testować silnik bez GUI. To jest najlepszy tryb do debugowania.

## 3. GUI

`Ds4.Gui` to prosty WinForms frontend:

- pole na dziedzinę,
- pole na kwerendę,
- przycisk „Oblicz”,
- panel wyniku z odpowiedzią i trace'em.

GUI nie zna szczegółów semantyki. Wywołuje tylko `Ds4Facade.Solve(...)`.
