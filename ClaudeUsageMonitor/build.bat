@echo off
rem Gera um unico ClaudeUsageMonitor.exe em .\publish (requer o .NET 8 SDK)
dotnet publish -c Release -r win-x64 -o publish
if errorlevel 1 exit /b 1
echo.
echo Pronto: %~dp0publish\ClaudeUsageMonitor.exe
