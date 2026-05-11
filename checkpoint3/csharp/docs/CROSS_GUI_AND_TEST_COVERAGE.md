# Cross GUI i pokrycie testami

Ta wersja zawiera poprawkę dla `Ds4.CrossGui`, czyli GUI opartego o Avalonię dla Windows, Linux i macOS.

## Poprawki GUI

- wybór innego przykładu z listy automatycznie ładuje jego domenę i kwerendę,
- przycisk `Wczytaj przykład` nadal działa ręcznie,
- pola tekstowe używają avaloniowych attached properties `ScrollViewer.SetHorizontalScrollBarVisibility` i `ScrollViewer.SetVerticalScrollBarVisibility`,
- GUI ładuje przykłady przez `Ds4Facade.ListExamples()` i `Ds4Facade.LoadExample()`, czyli z folderu `examples`, bez fallbacku hardcodowanego.

## Testy

Projekt `tests/Ds4.Tests` zawiera testy xUnit dla:

- parsera formuł,
- parsera dziedziny,
- parsera procesu,
- parsera kwerend,
- generowania stanów,
- `always`, `initially`, `after`, `observable after`,
- `causes`, `releases`, `impossible`, `noninertial`,
- konfliktów akcji złożonych,
- dekompozycji,
- procesu wielokrokowego,
- kwerend `possibly` i `necessary`,
- regresji `action causes q if p`,
- wszystkich przykładów z folderu `examples` względem plików `.expected`,
- statycznych elementów projektu,
- zachowania źródła `Ds4.CrossGui`.

Uruchamianie:

```powershell
dotnet test Ds4Reasoner.sln
```
