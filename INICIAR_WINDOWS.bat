@echo off
cd /d "%~dp0"
dotnet restore
if errorlevel 1 goto fallo
dotnet run
pause
exit /b
:fallo
echo No se pudo restaurar. Revisa que tengas el SDK .NET 8 e internet.
pause
