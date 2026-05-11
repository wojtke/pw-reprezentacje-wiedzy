# DS4 Reasoner C#

Projekt implementuje prosty reasoner dla języka akcji klasy DS4. Aplikacja pozwala opisywać dziedziny dynamiczne, wykonywać procesy akcji prostych i złożonych oraz sprawdzać kwerendy typu `possibly` i `necessary`.

Projekt zawiera:

- bibliotekę logiki `Ds4.Core`,
- aplikację konsolową `Ds4.Cli`,
- GUI Windows `Ds4.Gui`,
- GUI cross-platform `Ds4.CrossGui`,
- testy jednostkowe i integracyjne `Ds4.Tests`,
- przykłady domen i kwerend w folderze `examples`,
- gotowe pliki wykonywalne w folderze `release`.

## Struktura projektu

```text
.
├── Ds4Reasoner.sln
├── README.md
├── examples/
│   ├── *.domain
│   ├── *.query
│   └── *.expected
├── src/
│   ├── Ds4.Core/
│   ├── Ds4.Cli/
│   ├── Ds4.Gui/
│   └── Ds4.CrossGui/
├── tests/
│   └── Ds4.Tests/
└── release/
    ├── Ds4.Gui.exe
    ├── Ds4.CrossGui
    └── examples/

## Przykłady

Folder `examples` zawiera 40 przypadków:

- 23 przypadki z odpowiedzią TAK,
- 17 przypadków z odpowiedzią NIE.

Przykłady z prefiksem `tex` zostały przepisane z pliku `checkpoint2_v1.tex`. Przykłady z prefiksem `extra` są dodatkowymi przypadkami testowymi.

GUI nie korzysta już z hardcodowanych przykładów. Lista w GUI jest budowana z plików `.domain` i `.query` w folderze `examples`.

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
