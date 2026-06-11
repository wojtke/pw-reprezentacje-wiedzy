@echo off
dotnet publish src\Ds4.CrossGui\Ds4.CrossGui.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
pause
