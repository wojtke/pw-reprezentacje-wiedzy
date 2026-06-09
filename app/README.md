# DS4 Reasoner C#

Aplikacja demonstracyjna do projektu z Reprezentacji Wiedzy: procesy działań złożonych w klasie DS4.

## Co jest w środku

- `src/Ds4.Core` - parser, model, semantyka i ewaluacja kwerend.
- `src/Ds4.Cli` - wersja konsolowa do testów i automatyzacji.
- `src/Ds4.Gui` - proste GUI WinForms.
- `examples` - przykłady ładowane przez GUI i CLI.
- `tests/Ds4.Tests` - testy C# w xUnit.

## Przykłady

Folder `examples` zawiera 42 przypadki:

- 24 przypadki z odpowiedzią TAK,
- 18 przypadków z odpowiedzią NIE.

Przykłady z prefiksem `tex` zostały przepisane z pliku `checkpoint2_v1.tex`. Przykłady z prefiksem `extra` są dodatkowymi przypadkami testowymi.

GUI nie korzysta już z hardcodowanych przykładów. Lista w GUI jest budowana z plików `.domain` i `.query` w folderze `examples`.

Pliki `.domain` nie wymagają deklaracji `fluents` ani `actions`. Program zbiera fluenty z formuł oraz akcje z reguł typu `A causes ...`, `A releases ...`, `impossible A ...` i z kwerendy. Deklaracje `fluents` oraz `actions` są tylko opcjonalną wygodą dla starszych przykładów.

## Uruchomienie GUI

```powershell
dotnet run --project src\Ds4.Gui\Ds4.Gui.csproj
```

Albo kliknij `RUN_GUI.bat`.

## Uruchomienie CLI

```powershell
dotnet run --project src\Ds4.Cli\Ds4.Cli.csproj -- examples\tak_extra_01_simple_cause.domain examples\tak_extra_01_simple_cause.query
```

Lista przykładów:

```powershell
dotnet run --project src\Ds4.Cli\Ds4.Cli.csproj -- --examples
```

## Testy C#

Testy są w `tests/Ds4.Tests` i używają xUnit.

```powershell
dotnet restore
dotnet test Ds4Reasoner.sln
```

Albo kliknij `RUN_TESTS.bat`.

Testy sprawdzają parsery, formuły, model stanów, semantykę akcji prostych, konflikty, dekompozycje, procesy, kwerendy oraz wszystkie przykłady z folderu `examples`.

## Publikacja EXE

```powershell
dotnet publish src\Ds4.Gui\Ds4.Gui.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Do oddania prowadzącej skopiuj cały folder `publish`, nie tylko sam plik exe, ponieważ przykłady są kopiowane jako dane aplikacji.

## Linux GUI

Dla Linuxa użyj projektu `Ds4.CrossGui`, który bazuje na Avalonia UI:

```bash
dotnet run --project src/Ds4.CrossGui/Ds4.CrossGui.csproj
```

Publikacja Linux x64:

```bash
dotnet publish src/Ds4.CrossGui/Ds4.CrossGui.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

WinForms `Ds4.Gui` zostaje dla Windows. CLI `Ds4.Cli` działa cross-platform.
