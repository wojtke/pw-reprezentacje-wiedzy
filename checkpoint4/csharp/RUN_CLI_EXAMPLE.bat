@echo off
cd /d "%~dp0"
dotnet run --project src\Ds4.Cli\Ds4.Cli.csproj -- examples\tak_01_switches_necessary_alarm.domain examples\tak_01_switches_necessary_alarm.query
pause
