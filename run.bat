@echo off
setlocal

dotnet build MunicipalReporter.sln
if errorlevel 1 (
  echo Build failed. The application was not started.
  exit /b 1
)

start "" "bin\Debug\net472\MunicipalReporter.exe"
