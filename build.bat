@echo off
setlocal

if not exist bin\Debug mkdir bin\Debug

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe" ^
  /nologo ^
  /target:winexe ^
  /unsafe ^
  /out:bin\Debug\MunicipalReporter.exe ^
  /reference:System.dll ^
  /reference:System.Core.dll ^
  /reference:System.Drawing.dll ^
  /reference:System.Windows.Forms.dll ^
  Program.cs ^
  Forms\MainMenuForm.cs ^
  Forms\ReportIssueForm.cs ^
  Models\MunicipalIssue.cs ^
  Models\ReportAttachment.cs ^
  Services\ReportRepository.cs ^
  Services\ReportingProgressCalculator.cs ^
  Properties\AssemblyInfo.cs

if errorlevel 1 (
  echo Build failed.
  exit /b 1
)

echo Build succeeded. Run bin\Debug\MunicipalReporter.exe
