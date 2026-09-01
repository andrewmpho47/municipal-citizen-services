# Marking Guide

This file maps the application directly to the Part 1 Task 2 rubric.

## App Functionality: Task Presentation on Startup

- `Program.cs` starts the app with `MainMenuForm`.
- `MainMenuForm` displays three startup options:
  - Report Issues
  - Local Events and Announcements
  - Service Request Status
- Only `Report Issues` is enabled.
- The other two options are disabled because they are marked for later implementation in the brief.

Relevant file: `Forms/MainMenuForm.cs`

## App Functionality: Report Issues Task Implementation

The report form includes all required controls:

- Location input: `TextBox`
- Category selection: `ComboBox`
- Description input: `RichTextBox`
- Media attachment: `OpenFileDialog`
- Submit button: `Button`
- Engagement feature: `ProgressBar` and dynamic encouraging `Label`
- Navigation: `Back to Main Menu` button

The submit process validates required fields and shows success or warning messages using `MessageBox`.

Relevant file: `Forms/ReportIssueForm.cs`

## App Logic: Appropriate Data Structures

- User-reported issues are stored in `ReportRepository<MunicipalIssue>`.
- The repository internally uses `List<T>`.
- Attachments are stored in `List<ReportAttachment>`.

Relevant files:

- `Services/ReportRepository.cs`
- `Models/MunicipalIssue.cs`
- `Forms/ReportIssueForm.cs`

## Coding Standards: README Quality

The README explains:

- What the application does
- Features included
- Programming concepts covered
- How to compile
- How to run
- How to use the application
- VS Code troubleshooting

Relevant file: `README.md`

## Design Considerations

- The application uses a consistent color scheme across forms.
- Labels and buttons use clear wording.
- Feedback is provided through validation and success `MessageBox` dialogs.
- Forms use `TableLayoutPanel`, `FlowLayoutPanel`, docking, and minimum sizes to support resizing.

Relevant files:

- `Forms/MainMenuForm.cs`
- `Forms/ReportIssueForm.cs`

## Additional Requirements: Form Interactions and Data Handling

Event handlers are implemented for:

- Main menu report button
- Attachment button
- Submit button
- Back button
- Text changed events
- Category selection changed event

Data handling is implemented with a generic repository and lists.

Relevant files:

- `Forms/MainMenuForm.cs`
- `Forms/ReportIssueForm.cs`
- `Services/ReportRepository.cs`

## Programming Concepts Required by the User

- Arrays: `ReportIssueForm.cs`, `ReportingProgressCalculator.cs`
- Lists: `ReportIssueForm.cs`, `MunicipalIssue.cs`, `ReportRepository.cs`
- Pointers: `ReportingProgressCalculator.cs`
- Generics: `ReportRepository<T>`
- Overloading: `MunicipalIssue` constructors, `AddAttachment` methods, `ReportRepository.Add` methods
- Recursion: `ReportingProgressCalculator.CountCompletedRecursive`
