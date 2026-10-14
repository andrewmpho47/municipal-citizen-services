# Municipal Citizen Services

A .NET Framework 4.7.2 Windows Forms application for exploring municipal services in South Africa. It includes issue reporting from Part 1 and a searchable Local Events and Announcements feature for Part 2.

## Features

- **Report Issues:** submit a location, category, description, and optional attachments.
- **View Submitted Issues:** view reports submitted during the current application session.
- **Local Events and Announcements:** browse upcoming sample events, search by category and date range, and view event details.
- **Recommendations:** receive event suggestions based on categories explicitly searched during the current session.
- **Service Request Status:** displays a “Coming Soon” dialog; this feature is scheduled for Task 3.
- A supplied municipal-services banner brands the main menu and events page.

> The events in this project are illustrative sample data, not a live municipal feed or official event calendar. Issue reports, search history, and preferences are held in memory and are cleared when the application closes.

## Requirements

- Windows (the application uses Windows Forms).
- .NET SDK with support for the project target, .NET Framework 4.7.2. The project references the .NET Framework reference assemblies package so `dotnet build` can restore them when needed.
- Alternatively, Visual Studio with the .NET Framework 4.7.2 targeting pack.

## Build and run

From the project directory, build and launch using:

```bat
run.bat
```

To build with the .NET SDK:

```powershell
dotnet build MunicipalReporter.sln
```

The SDK build creates `bin\Debug\net472\MunicipalReporter.exe`. Launch it with:

```powershell
.\bin\Debug\net472\MunicipalReporter.exe
```

The project also includes a Windows Framework compiler build script:

```bat
build.bat
bin\Debug\MunicipalReporter.exe
```

The script enables unsafe code for the Part 1 pointer example and compiles the Part 2 form, model, and event repository.
Both build paths copy `Assets\MunicipalCitizensServicesBanner.png` beside the executable for the branded forms.

## How to use

### Report an issue

1. Choose **Report Issues**.
2. Enter the location, select a category, and describe the issue.
3. Optionally attach supporting images or documents.
4. Submit the report. A confirmation message displays its reference number.
5. Choose **View Submitted Issues** from the main menu to see reports submitted during this run.

### Find an event or announcement

1. Choose **Local Events and Announcements**.
2. Select a category or leave **All categories** selected.
3. Optionally check the **From** and/or **To** date fields and choose dates. The date range is inclusive.
4. Choose **Search**. Choose **Clear filters** to return to the upcoming-events list.
5. Select an event in the results or recommendations to view its details.
6. Search history and recommendations update during the current application session. Explicit category searches inform category preferences.

## Part 2 data structures and algorithms

The event feature uses these structures to organize events, support searches, and recommend relevant results:

| Structure | Use |
| --- | --- |
| `SortedDictionary<DateTime, List<MunicipalEvent>>` | Groups events by date so upcoming events can be traversed chronologically. The per-date list orders multiple events on the same day. |
| `Dictionary<string, MunicipalEvent>` | Looks up an event by its unique ID without scanning the complete event list. |
| `Dictionary<string, HashSet<string>>` | Maps a category to its event IDs; category searches then inspect only that category’s events. |
| `Dictionary<string, int>` | Counts explicit searches by category to represent the user's current-session interests. |
| `HashSet<string>` | Stores unique categories and category event IDs, and excludes already displayed events from suggestions. |
| `Queue<MunicipalEvent>` | Provides the preordered upcoming-event sequence in first-in, first-out order. |
| `Stack<string>` | Keeps searches in last-in, first-out order so the most recent searches can be displayed first. |
| `EventRecommendationPriorityQueue` (binary max-heap) | Ranks upcoming suggestions. Insertions and removals take O(log n); the highest-priority candidate is returned first. |

The recommendation score for an event is:

```text
(category search count * 100) + max(0, 30 - days until event)
```

This favors categories the resident has explicitly searched for, then favors nearer dates when interest is equal. Already displayed search results are omitted where other suggestions are available.

## Part 1 programming concepts

- **Arrays:** issue categories and engagement messages.
- **Lists:** report attachments, submitted issues, and events on the same date.
- **Pointers:** the reporting progress calculator includes a pointer-based array sum.
- **Generics:** `ReportRepository<T>` stores reports.
- **Overloading:** issue constructors, attachment methods, and repository add methods.
- **Recursion:** `ReportingProgressCalculator.CountCompletedRecursive` counts completed form sections.

See [MARKING_GUIDE.md](./MARKING_GUIDE.md) for the project-to-rubric mapping.
