# Testy C#

Projekt zawiera testy w `tests/Ds4.Tests`. Są napisane w xUnit i uruchamiane przez standardowe `dotnet test`.

Zakres testów:

- parser dziedziny i kwerend,
- ewaluacja formuł logicznych,
- generowanie stanów `Sigma` i `Sigma0`,
- semantyka akcji prostych,
- `impossible`, `releases`, inercja i ramifikacje przez `always`,
- konflikty akcji złożonych,
- dekompozycje,
- procesy i blokujące się ścieżki,
- kwerendy `possibly` i `necessary`,
- wszystkie przykłady z folderu `examples`, razem z plikami `.expected`,
- kontrola struktury projektu, w tym brak znaków em dash.

Uruchomienie:

```powershell
dotnet restore
dotnet test Ds4Reasoner.sln
```

Można też kliknąć `RUN_TESTS.bat`.


## xUnit imports

The test project contains `tests/Ds4.Tests/GlobalUsings.cs` with:

```csharp
global using Xunit;
```

This makes `[Fact]`, `[Theory]`, `[InlineData]`, `[MemberData]` and `Assert.*` available in all test files.
