@echo off
setlocal

if not exist bin\Debug mkdir bin\Debug
if not exist bin\Debug\Assets mkdir bin\Debug\Assets

copy /Y Assets\MunicipalCitizensServicesBanner.png bin\Debug\Assets\MunicipalCitizensServicesBanner.png >nul
if errorlevel 1 (
  echo Could not copy the municipal services banner.
  exit /b 1
)

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
  Forms\IssueListForm.cs ^
  Forms\EventAnnouncementsForm.cs ^
  Models\MunicipalIssue.cs ^
  Models\MunicipalEvent.cs ^
  Models\ReportAttachment.cs ^
  Services\ReportRepository.cs ^
  Services\EventRepository.cs ^
  Services\MunicipalBranding.cs ^
  Services\ReportingProgressCalculator.cs ^
  Properties\AssemblyInfo.cs

if errorlevel 1 (
  echo Build failed.
  exit /b 1
)

echo Build succeeded. Run bin\Debug\MunicipalReporter.exe
