# ReferenceManager Dataset

## Contents

```
dataset/
  ReferenceManager/    C#/.NET 9 web API developed entirely through agentic coding
  ReferenceManagerUI/  Single-page frontend for live demonstration
  demonstration/       Transcripts of all 10 evolution steps
  README.md            This file
```

## ReferenceManager

A web API for managing research paper references, built with .NET 9, Entity Framework Core, and SQLite. Developed entirely through agentic coding using Claude Code.

**Final state**: 85 source files, 2 971 lines of C# application code, 25 features across four top-level concerns.

### Build and run

```bash
cd dataset/ReferenceManager
dotnet run
# API + Scalar UI available at http://localhost:5062/scalar/v1
```

## ReferenceManagerUI

A single-file browser frontend (`ReferenceManagerUI/index.html`) for interacting with the API. No build step or Node.js required — open the file directly in a browser.

### Run the demo

1. Start the API:

```bash
cd dataset/ReferenceManager
dotnet run
# API running at http://localhost:5062
```

2. Open the frontend in a browser:

```
dataset/ReferenceManagerUI/index.html
```

The sidebar shows a green **Connected** indicator when the API is reachable. If the API runs on a different port, click the status indicator to change the URL.

The UI covers all API features: paper CRUD, full-text search, BibTeX import/export, group management, and author management.

---

### Run tests

```bash
cd dataset/ReferenceManager
dotnet test
```

## Demonstration transcripts

`demonstration/` contains one file per evolution step — verbatim transcripts of the agentic coding session, showing the developer prompt, Claude Code's tool calls and output, and the automatic feature model update after each coding cycle.

| File | Evolution | Feature model change | Type |
|------|-----------|---------------------|------|
| `evolution-point1.txt` | Ev. 1 | + `Database`, `ApiDocs`, `Papers` | Add |
| `evolution-point2.txt` | Ev. 2 | + `Collections`, `Versioning` | Add |
| `evolution-point3.txt` | Ev. 3 | + `Tags` | Add |
| `evolution-point4.txt` | Ev. 4 | + `Favorites` | Add |
| `evolution-point5.txt` | Ev. 5 | `Tags` + `Collections` → `Groups` | Merge |
| `evolution-point6.txt` | Ev. 6 | + `Papers.ImportBibTex` | Add |
| `evolution-point7.txt` | Ev. 7 | + `Papers.ExportBibTex` | Add |
| `evolution-point8.txt` | Ev. 8 | `Papers.Authors` → `Authors` | Split |
| `evolution-point9.txt` | Ev. 9 | + `Papers.SearchPapers` | Add |
| `evolution-point10.txt` | Ev. 10 | − `Favorites` | Remove |

`demonstration/prompts.md` lists all 27 developer prompts across the 10 evolution steps.  
`demonstration/result-stats.md` reports final cloc statistics for the codebase.

## Redaction note

Personal and organizational identifiers have been redacted: the developer's name is replaced with *A. Developer*, the employing organization with *The Company*, and local file system paths are relative.
