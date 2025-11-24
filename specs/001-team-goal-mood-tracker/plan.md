# Implementation Plan: Team Daily Goal Tracker with Mood Sync

**Branch**: `001-team-goal-mood-tracker` | **Date**: 2025-11-24 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-team-goal-mood-tracker/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Build a .NET 8 Web API backend that enables small teams to track daily goals and monitor team morale through mood tracking. The system provides REST API endpoints for viewing all team members with their goals and mood status (P1), updating individual mood (P2), creating and managing goals (P3), and deleting goals (P4). Uses SQLite for local storage with Dapper ORM, following MVC architecture with Repository pattern. Frontend integration via CORS-enabled endpoints with standardized error responses.

## Technical Context

**Language/Version**: C# / .NET 8  
**Primary Dependencies**: Dapper (ORM), Swashbuckle.AspNetCore (Swagger/OpenAPI), Microsoft.Data.Sqlite  
**Storage**: SQLite (local file-based database, gitignored with seed script)  
**Testing**: No tests for v1 (explicitly stated in requirements)  
**Target Platform**: Cross-platform (.NET 8 runtime - Windows/Linux/macOS)  
**Project Type**: Web API (backend only, frontend already implemented)  
**Performance Goals**: <2s for GET /api/members (50 members × 10 goals), <1s for POST operations  
**Constraints**: Last-write-wins for concurrent mood updates, no pagination (small team focus), CORS enabled for all domains  
**Scale/Scope**: 50 team members maximum, 10 goals per member, 5 API endpoints, no authentication

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. .NET 8 & Dapper ORM ✅ PASS
- Using .NET 8 Web API framework
- Using Dapper exclusively for data access
- No Entity Framework usage

### II. SQLite Local Database ✅ PASS
- SQLite database file (local storage)
- Database file gitignored per clarifications
- Seed data script planned for reproducibility

### III. MVC Folder Structure ✅ PASS
- Controllers/ for API endpoints
- Services/ for business logic
- Models/ with Requests/, Responses/, DBOs/ subfolders
- Repositories/ for Dapper data access
- Helpers/ for utilities (if needed)
- Filters/ for action/exception filters (if needed)

### IV. Naming Conventions ✅ PASS
- Request models: UpdateMoodRequest, CreateGoalRequest, UpdateGoalRequest, DeleteGoalRequest
- Response models: MemberResponse, GoalResponse, ErrorResponse
- DBO models: TeamMemberDBO, GoalDBO

### V. CORS Policy ✅ PASS
- CORS configured to allow all domains (FR-026)
- Supports existing frontend integration

**Gate Status**: ✅ **ALL GATES PASSED** - Proceed to Phase 0

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
VibeLoopBE/                    # .NET 8 Web API project
├── Controllers/               # API endpoints
│   ├── MembersController.cs  # GET /api/members, POST /api/members/mood
│   └── GoalsController.cs    # POST /api/goals, /api/goals/update, /api/goals/delete
├── Services/                  # Business logic
│   ├── IMemberService.cs
│   ├── MemberService.cs
│   ├── IGoalService.cs
│   └── GoalService.cs
├── Repositories/              # Data access with Dapper
│   ├── IMemberRepository.cs
│   ├── MemberRepository.cs
│   ├── IGoalRepository.cs
│   └── GoalRepository.cs
├── Models/
│   ├── Requests/              # API request models
│   │   ├── UpdateMoodRequest.cs
│   │   ├── CreateGoalRequest.cs
│   │   ├── UpdateGoalRequest.cs
│   │   └── DeleteGoalRequest.cs
│   ├── Responses/             # API response models
│   │   ├── MemberResponse.cs
│   │   ├── GoalResponse.cs
│   │   └── ErrorResponse.cs
│   └── DBOs/                  # Database objects
│       ├── TeamMemberDBO.cs
│       └── GoalDBO.cs
├── Helpers/                   # Utilities (optional)
│   └── DatabaseHelper.cs     # Connection management
├── Filters/                   # Exception handling (optional)
│   └── GlobalExceptionFilter.cs
├── Data/
│   ├── schema.sql            # Database schema
│   └── seed.sql              # Seed data script
├── vibeloop.db               # SQLite database (gitignored)
├── Program.cs                # App configuration & DI
├── appsettings.json          # Configuration
└── VibeLoopBE.csproj         # Project file
```

**Structure Decision**: Using .NET MVC Web API structure (Option 3) per constitution. All code under VibeLoopBE/ project directory following MVC pattern with Repository layer for data access.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

**No violations** - All constitution principles followed. No complexity justification required.

---

## Phase 0: Outline & Research

### Research Completed ✅

**File**: `research.md`

**Key Decisions Documented**:
1. .NET 8 Web API with Dapper (mandated by constitution)
2. SQLite local database (constitution requirement)
3. MVC with Repository pattern (constitution standard)
4. Error response format: `{ "error": "CODE", "message": "..." }` (clarified)
5. Concurrent mood updates: Last-write-wins (clarified)
6. Database management: Gitignored with seed script (clarified)

**Best Practices Researched**:
- Dapper connection management with dependency injection
- SQLite date/time handling (ISO 8601 TEXT format)
- CORS configuration for all domains
- Multi-mapping for JOIN queries

**Unknowns Resolved**: All - no NEEDS CLARIFICATION markers remain

---

## Phase 1: Design & Contracts

### 1. Data Model ✅

**File**: `data-model.md`

**Entities Defined**:
- **TeamMember**: Id, Name, Mood (nullable), MoodUpdatedAt (nullable)
- **Goal**: Id, MemberId (FK), Description, Completed, CreatedAt, Date

**Database Schema**:
- SQLite tables with CHECK constraints for validation
- Foreign key with CASCADE delete
- Indexes on MemberId and Date

**C# Models**:
- DBOs for database mapping (TeamMemberDBO, GoalDBO)
- Request models (UpdateMoodRequest, CreateGoalRequest, etc.)
- Response models (MemberResponse, GoalResponse, ErrorResponse)

### 2. API Contracts ✅

**File**: `contracts/openapi.yaml`

**Endpoints Specified**:
1. GET `/api/members` - Retrieve all members with goals
2. POST `/api/members/mood` - Update member mood
3. POST `/api/goals` - Create new goal
4. POST `/api/goals/update` - Update goal completion
5. POST `/api/goals/delete` - Delete goal

**Contract Features**:
- Full OpenAPI 3.0.3 specification
- Request/response schemas with examples
- Error response format standardized
- Validation rules documented (description length, mood values, date format)

### 3. Quickstart Guide ✅

**File**: `quickstart.md`

**Contents**:
- Setup instructions (dependencies, database initialization)
- Running the API (dotnet run, Swagger UI)
- Quick API test examples with curl commands
- Project structure overview
- Development workflow (add members, view data, reset database)
- Troubleshooting guide (port conflicts, database locks, CORS errors)

### 4. Agent Context Update ✅

**Updated**: `.github/agents/copilot-instructions.md`

**Added Technology**:
- Language: C# / .NET 8
- Framework: Dapper, Swashbuckle.AspNetCore, Microsoft.Data.Sqlite
- Database: SQLite (gitignored with seed script)
- Project Type: Web API

### Constitution Re-Check ✅

**Status**: ✅ **ALL GATES STILL PASSED**

- .NET 8 & Dapper ORM: Compliant (no EF usage)
- SQLite Local Database: Compliant (gitignored, seed script planned)
- MVC Folder Structure: Compliant (Controllers/Services/Repositories/Models)
- Naming Conventions: Compliant (Request/Response/DBO patterns followed)
- CORS Policy: Compliant (allow all domains configured)

**Design Integrity**: No violations introduced during Phase 1 design.

---

## Next Steps

### Phase 2: Implementation Tasks (Run `/speckit.tasks`)

The planning phase is complete. To begin implementation:

```bash
# Generate task breakdown
/speckit.tasks
```

This will create `tasks.md` with:
- Phase 1: Setup (project structure, dependencies)
- Phase 2: Foundation (database schema, seed data)
- Phase 3-6: User story implementation (P1-P4)
- Phase 7: Polish (error handling, CORS, Swagger)

### Recommended Implementation Order

1. **Setup Foundation** (Tasks.md Phase 1-2)
   - Configure Dapper and SQLite
   - Create database schema and seed script
   - Setup dependency injection

2. **Implement P1: View Team Dashboard** (Read-only MVP)
   - MembersController GET endpoint
   - Repository layer with multi-mapping JOIN
   - Test with seed data

3. **Implement P2: Mood Tracking**
   - MembersController POST /mood endpoint
   - Last-write-wins logic in service layer
   - Update existing member data

4. **Implement P3: Goal Management**
   - GoalsController POST endpoints (create, update)
   - Goal validation logic
   - Test completion percentage calculation

5. **Implement P4: Goal Deletion**
   - GoalsController POST /delete endpoint
   - CASCADE delete testing

### Documentation Generated

| File | Purpose | Status |
|------|---------|--------|
| `plan.md` | This file - technical plan | ✅ Complete |
| `research.md` | Technology decisions | ✅ Complete |
| `data-model.md` | Database schema & entities | ✅ Complete |
| `contracts/openapi.yaml` | API specification | ✅ Complete |
| `quickstart.md` | Setup & testing guide | ✅ Complete |
| `tasks.md` | Implementation tasks | ⏭️ Next: Run `/speckit.tasks` |

---

## Summary

**Feature**: Team Daily Goal Tracker with Mood Sync (v1)  
**Branch**: `001-team-goal-mood-tracker`  
**Architecture**: .NET 8 Web API + Dapper + SQLite  
**Pattern**: MVC with Repository layer  
**Endpoints**: 5 REST endpoints (1 GET, 4 POST)  
**Entities**: TeamMember, Goal (2 tables)  
**Constitution Compliance**: ✅ 100% (all 5 principles followed)  
**Phase Status**: Phase 0 ✅ | Phase 1 ✅ | Phase 2 ⏭️ (Ready for `/speckit.tasks`)
