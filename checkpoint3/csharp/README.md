# DS4 Reasoner - C# prototype

## Najszybsze uruchomienie w Visual Studio

Otwórz `Ds4Reasoner.sln`, potem w **Solution Explorer** kliknij prawym na `Ds4.Gui` i wybierz **Set as Startup Project**. Nie uruchamiaj `Ds4.Core`, bo to jest biblioteka klas, nie program wykonywalny.

Uruchamialne projekty to:

- `Ds4.Gui` - aplikacja okienkowa WinForms,
- `Ds4.Cli` - aplikacja konsolowa,
- `Ds4.Core` - biblioteka z logiką DS4.

Możesz też kliknąć `RUN_GUI.bat`.

---

To jest prototyp aplikacji do projektu **Procesy działań złożonych** z Reprezentacji Wiedzy.
Kod jest przepisany jako mały, samodzielny projekt C#/.NET: core + parser + ewaluator + CLI + prosty WinForms GUI.

## Struktura

```text
src/Ds4.Core   - model domeny, formuły, parser, semantyka, facade
src/Ds4.Cli    - proste uruchamianie z terminala
src/Ds4.Gui    - proste GUI WinForms dla Windows
examples/      - przykładowe dziedziny i kwerendy; GUI ładuje listę z tego folderu
```

## Uruchomienie CLI

```bash
dotnet run --project src/Ds4.Cli -- examples/tak_01_switches_necessary_alarm.domain examples/tak_01_switches_necessary_alarm.query
```

Lista przykładów z folderu `examples`:

```bash
dotnet run --project src/Ds4.Cli -- --examples
```

Podgląd przykładu:

```bash
dotnet run --project src/Ds4.Cli -- --example tak_01_switches_necessary_alarm
```

## Uruchomienie GUI

Na Windowsie:

```bash
dotnet run --project src/Ds4.Gui
```

## Publikacja jednoplikowego EXE

Przykładowo dla GUI na Windows x64:

```bash
dotnet publish src/Ds4.Gui -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## Przykłady w GUI

GUI ładuje przypadki z folderu `examples`. Każdy przypadek to para plików `<id>.domain` i `<id>.query`. Aktualnie folder zawiera trzy przypadki z odpowiedzią TAK i trzy z odpowiedzią NIE. Szczegóły są w `docs/EXAMPLES_FROM_FOLDER.md`.

## Obsługiwana składnia dziedziny

```text
fluents p, q, r
actions A, B
always p -> q
initially !p and q
noninertial q
A causes p if q
A releases r if true
impossible A if !q
```

Obsługiwane spójniki formuł:

```text
! albo not
& albo and
| albo or
->
<->
(...)
true, false
```

## Obsługiwana składnia kwerend

```text
possibly executable after A; {B,C}
necessary executable after A; {B,C}
possibly p after A; {B,C}
necessary p after A; {B,C}
```

Proces składa się z kroków rozdzielonych średnikiem. Krok może być akcją prostą `A` albo akcją złożoną `{A,B}`.

## Co implementuje core

- generację stanów dopuszczalnych `Σ` przez filtrację `always`,
- generację stanów początkowych `Σ₀` przez `initially`,
- `Res(A, σ)` dla akcji prostej: `impossible`, aktywne efekty, `releases`, minimalizacja `New`,
- wykrywanie konfliktów dla akcji złożonych,
- generowanie maksymalnych bezkonfliktowych dekompozycji,
- `Res(𝔄, σ)` przez sumę wyników po dekompozycjach,
- budowę drzewa wykonania procesu,
- cztery typy kwerend: `possibly/necessary executable` oraz `possibly/necessary γ after P`.

## Uwaga uczciwa

To jest prototyp projektowy, nie przemysłowy solver logiczny. Świadomie używa pełnej eksploracji stanów i podzbiorów akcji, bo przykłady projektowe są małe, a takie podejście najczytelniej odpowiada semantyce z dokumentu.
