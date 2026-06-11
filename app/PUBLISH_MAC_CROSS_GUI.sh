#!/usr/bin/env bash
set -euo pipefail
dotnet publish src/Ds4.CrossGui/Ds4.CrossGui.csproj -c Release -r osx-arm64 --self-contained true /p:PublishSingleFile=true
