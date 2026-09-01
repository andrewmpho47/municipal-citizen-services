# Municipal Reporter

Municipal Reporter is a .NET Framework Windows Forms application for citizen reporting of municipal issues and service requests.

## Features

- Main menu with three municipal service options.
- Report Issues is enabled.
- Local Events and Announcements is disabled for future implementation.
- Service Request Status is disabled for future implementation.
- Report form captures location, category, detailed description, and optional file attachments.
- OpenFileDialog supports attaching images and documents.
- ProgressBar and encouraging messages guide the user through the reporting process.
- MessageBox feedback confirms success or explains missing required fields.
- `MARKING_GUIDE.md` maps the code directly to the rubric.

## Programming Concepts Covered

- Arrays: categories and engagement messages are stored in arrays.
- Lists: attachments and submitted issues are stored in `List<T>` collections.
- Pointers: reporting progress includes an unsafe pointer-based array sum in `ReportingProgressCalculator`.
- Generics: `ReportRepository<T>` stores reported issues using a generic class.
- Overloading: `MunicipalIssue`, `ReportRepository`, and attachment methods use overloaded constructors/methods.
- Recursion: `ReportingProgressCalculator.CountCompletedRecursive` recursively counts completed form sections.

## Compile and Run

### Visual Studio

1. Open `MunicipalReporter.sln` in Visual Studio.
2. Ensure the `.NET Framework 4.7.2` targeting pack is installed.
3. Build the solution using `Build > Build Solution`.
4. Run the program using `Debug > Start Debugging` or press `F5`.

Because the project demonstrates pointers, unsafe code is enabled in the project settings.

The project also includes `Microsoft.NETFramework.ReferenceAssemblies.net472` so `dotnet build` can restore the .NET Framework reference assemblies if the local targeting pack is missing.

### Command Line Backup

If Visual Studio targeting packs are not installed, run:

```bat
build.bat
bin\Debug\MunicipalReporter.exe
```

The build script uses the .NET Framework compiler included with Windows and enables unsafe code for the pointer demonstration.

For the SDK-style project output, `dotnet build` creates the executable in `bin\Debug\net472`.

### Running from PowerShell

For this .NET Framework Windows Forms project, use:

```powershell
dotnet build
.\bin\Debug\net472\MunicipalReporter.exe
```

or simply:

```powershell
.\run.bat
```

You can also use:

```powershell
dotnet run
```

The project file is SDK-style so `dotnet run` launches the Windows Forms application in the same spirit as Visual Studio's Start button.

If VS Code shows `command 'csdevkit.debug.noDebugFileLaunch' not found`, use `Terminal > Run Task > run app`. That runs the Windows Forms executable directly and does not depend on C# Dev Kit debugging.

## How to Use

1. Launch the application.
2. Click `Report Issues`.
3. Enter the issue location.
4. Select a category such as sanitation, roads, or utilities.
5. Type a detailed description of the issue.
6. Optionally click `Attach Image or Document` and choose supporting files.
7. Watch the progress bar and engagement message update as the report is completed.
8. Click `Submit`.
9. The application displays a success message with a reference number.
10. Use `Back to Main Menu` to return to the startup menu.
