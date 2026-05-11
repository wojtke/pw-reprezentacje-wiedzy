# Testy i przykłady

Ta wersja projektu ma rozbudowany folder `examples` i testy C# w xUnit.

## Przykłady

Liczba przykładów: 42.

- TAK: 24
- NIE: 18

Każdy przykład ma trzy pliki:

- `.domain` - opis dziedziny,
- `.query` - pojedyncza kwerenda,
- `.expected` - oracle dla testów.

GUI czyta tylko `.domain` i `.query`. Pliki `.expected` są potrzebne testom.

## Testy

Testy znajdują się w `tests/Ds4.Tests`. Są podzielone według obszarów projektu:

- `FormulaTests.cs`,
- `ModelTests.cs`,
- `ParserTests.cs`,
- `StateAndModelBuilderTests.cs`,
- `SimpleActionEngineTests.cs`,
- `CompositeActionTests.cs`,
- `ProcessAndQueryTests.cs`,
- `FacadeAndExamplesTests.cs`,
- `ConditionalCauseRegressionTests.cs`,
- `FullSemanticsIntegrationTests.cs`.

Zakres testów:

- składnia formuł,
- parsowanie dziedziny,
- parsowanie kwerend,
- generacja `Sigma` i `Sigma0`,
- działanie `causes`, `releases`, `impossible`, `always`, `noninertial`,
- konflikty i dekompozycje,
- ewaluacja procesów,
- kwerendy `possibly` i `necessary`,
- zgodność wszystkich przykładów z plikami `.expected`.

Uruchomienie:

```powershell
dotnet restore
dotnet test Ds4Reasoner.sln
```
