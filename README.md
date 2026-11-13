# Municipal Citizen Services

A .NET Framework 4.7.2 Windows Forms application for exploring municipal services in South Africa. It includes issue reporting from Part 1 and a searchable Local Events and Announcements feature for Part 2.

## Login credentials

Use these demo credentials on the startup login screen:

| Role | Username | Password |
| --- | --- | --- |
| Citizen | `citizen` | `Citizen@123` |
| Admin | `admin` | `Admin@123` |

These are shared, hard-coded coursework accounts, not production credentials.

## Features

- **Report Issues:** submit a location, category, description, and optional attachments.
- **View Submitted Issues:** municipal admins can review reports submitted during the current application session.
- **Local Events and Announcements:** browse and search upcoming events, view event details, and add announcements.
- **Recommendations:** receive event suggestions based on categories explicitly searched during the current session.
- **Service Request Status:** citizens can browse their submitted issues and see status updates; municipal admins can review and advance requests through the approval workflow.
- **Role-based sign-in:** choose Citizen or Admin at startup; each role has different menu and workflow privileges.
- A supplied municipal-services banner brands the main menu and events page.

> Events in this project include illustrative sample data and locally added announcements; they are not a live municipal feed. Issue reports, status history, announcements, search history, and preferences are held in memory and are cleared when the application closes. The hard-coded sign-in accounts are for coursework demonstration only and are not production authentication.

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

The script enables unsafe code for the Part 1 pointer example and compiles the issue, service-request, and events features.
Both build paths copy `Assets\MunicipalCitizensServicesBanner.png` beside the executable for the branded forms.

## How to use

### Report an issue

1. At startup, choose **Citizen** and sign in with username `citizen` and password `Citizen@123`.
2. Choose **Report Issues**.
3. Enter the location, select a category, and describe the issue.
4. Optionally attach supporting images or documents.
5. Submit the report. A confirmation message explains that it is pending municipal review.

### Find an event or announcement

1. Choose **Local Events and Announcements**.
2. Select a category, type one into the category field (with suggestions as you type), or leave **All categories** selected.
3. Optionally check the **From** and/or **To** date fields and choose dates. The date range is inclusive.
4. Choose **Search**. Choose **Clear filters** to return to the upcoming-events list.
5. Select an event in the results or recommendations to view its details.
6. To add an announcement, sign in as Admin (`admin` / `Admin@123`), choose **Manage Events and Announcements**, enter the title, category, location, future date/time, and description, then publish it. Citizens can browse published announcements but cannot create them.
7. Search history and recommendations update during the current application session. Explicit category searches inform category preferences.

### Track and review a service request

1. Submit an issue while signed in as Citizen.
2. While signed in as Citizen, choose **Track Service Request** to browse submitted issues. Select an issue to see its current status and update history; no reference-number lookup is required.
3. Sign in as Admin (`admin` / `Admin@123`) and choose **Review Service Requests**. Select a report and approve or reject it directly, or move it to **Under Review** before deciding. Approved requests can move to **In Progress** and then **Resolved**.
4. Before applying a status change, the admin confirms the decision in a dialog. A confirmation message appears after the update. Citizens cannot open the admin request list or change statuses. Admins cannot submit citizen reports. Invalid status transitions are rejected and each accepted change appears in the history.

### Demo accounts

| Role | Username | Password |
| --- | --- | --- |
| Citizen | `citizen` | `Citizen@123` |
| Admin | `admin` | `Admin@123` |

The login is shown every time the application starts. Use **Switch User / Log Out** on the main menu to return to login and sign in with the other role; reports and announcements remain available for the current app session. The citizen account is shared, and there are no individual citizen accounts. Credentials are stored in the application source; this is a demonstration of role-based access, not secure authentication. Do not use these credentials or this login design for a real deployment.

### Final submission documents

- [Project completion report](./Project_Completion_Report.docx) covers the project from Part 1 through Task 3, including the implementation, data structures, testing evidence, recommendations, limitations, and references.
- [Combined municipal service overview](./Municipal_Citizen_Services_Combined_Report.docx) presents Parts 1, 2, and 3 together for municipal review, with the final feature screenshots and prototype-data notice.
- [Updates based on feedback](./Updates_Based_on_Feedback.md) records the lecturer feedback quoted in the Part 2 evidence report separately from later usability feedback.

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

## Task 3 service-request structures

- **AVL binary search tree:** `ServiceRequestRepository` indexes reports by reference number. AVL rotations keep the tree balanced as reports are added, so lookups and inserts remain O(log n); in-order traversal supplies the admin list.
- **Directed status graph:** the repository models valid status changes as directed edges. A breadth-first traversal returns the workflow path shown to residents. The admin controls only enable transitions connected to the selected request's current state.
- **Status history:** each accepted transition is timestamped on the request and is displayed to both end users and staff.
- **Priority queue (binary max-heap):** event recommendations use a heap to return the highest-scoring suggestions first.

The status graph permits `Submitted -> Under Review`, `Submitted -> Approved`, or `Submitted -> Rejected`; `Under Review` can move to `Approved` or `Rejected`; and `Approved -> In Progress -> Resolved`. Rejected and Resolved are terminal states. Data remains in memory for this coursework application.

**Coverage note:** the current implementation uses an AVL tree (which is also a binary search tree) and a binary max-heap. It does not currently implement a separate red-black tree or a minimum-spanning-tree algorithm. The current workflow graph describes request-status transitions; it is not a geographic municipal road network.

## Part 1 programming concepts

- **Arrays:** issue categories and engagement messages.
- **Lists:** report attachments, submitted issues, and events on the same date.
- **Pointers:** the reporting progress calculator includes a pointer-based array sum.
- **Generics:** `ReportRepository<T>` stores reports.
- **Overloading:** issue constructors, attachment methods, and repository add methods.
- **Recursion:** `ReportingProgressCalculator.CountCompletedRecursive` counts completed form sections.

See [MARKING_GUIDE.md](./MARKING_GUIDE.md) for the project-to-rubric mapping.
