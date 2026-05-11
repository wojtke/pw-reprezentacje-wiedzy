# GUI na Linuxie

Czyste .NET MAUI nie jest tutaj najlepszym wyborem, bo oficjalny MAUI desktop nie wspiera Linuxa jako standardowego targetu. Dlatego w projekcie są teraz dwa GUI:

- `Ds4.Gui`: WinForms dla Windows.
- `Ds4.CrossGui`: Avalonia UI dla Windows, Linux i macOS.

Logika aplikacji jest wspólna w `Ds4.Core`, więc wyniki powinny być takie same w CLI, WinForms i Avalonia.

## Uruchomienie na Linuxie

```bash
dotnet restore
dotnet run --project src/Ds4.CrossGui/Ds4.CrossGui.csproj
```

## Publikacja na Linux x64

```bash
dotnet publish src/Ds4.CrossGui/Ds4.CrossGui.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

Gotowy plik będzie w:

```text
src/Ds4.CrossGui/bin/Release/net8.0/linux-x64/publish/
```

Przekazuj cały folder `publish`, bo zawiera też folder `examples`.

## Publikacja na Windows x64

```powershell
dotnet publish src\Ds4.CrossGui\Ds4.CrossGui.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```
