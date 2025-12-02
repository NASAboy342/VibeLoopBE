# Implementation Plan: Goal Background Color

**Branch**: `002-goal-color` | **Date**: December 2, 2025 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-goal-color/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Add background color support to goals stored as RGB string format. Each goal will have an optional color field that accepts "rgb(R, G, B)" format, defaults to white "rgb(255, 255, 255)", validates RGB values (0-255), normalizes spacing before storage, and returns color in all API responses. Includes database migration to backfill existing goals with default white color.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Dapper 2.1.28, Microsoft.Data.Sqlite 8.0.0, Swashbuckle.AspNetCore 6.6.2  
**Storage**: SQLite (local file-based database)  
**Testing**: xUnit or NUnit (to be determined)  
**Target Platform**: Cross-platform (.NET 8 Web API)  
**Project Type**: Web API (MVC pattern)  
**Performance Goals**: 1000 concurrent goal operations without degradation (per spec SC-006)  
**Constraints**: <5 seconds for goal creation with color (per spec SC-001), HTTP 400 validation errors with clear messages  
**Scale/Scope**: Team goal tracking application, extending existing Goals table with color field

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Initial Check (Before Phase 0)

| Principle | Compliance | Notes |
|-----------|------------|-------|
| I. .NET 8 & Dapper ORM | ✅ PASS | Using existing .NET 8 Web API with Dapper for data access |
| II. SQLite Local Database | ✅ PASS | Extending existing SQLite database with new column |
| III. MVC Folder Structure | ✅ PASS | Following Controllers → Services → Repositories → Database flow |
| IV. Naming Conventions | ✅ PASS | Will use ColorBgColor property in GoalDBO, optional field in CreateGoalRequest/UpdateGoalRequest |
| V. CORS Policy | ✅ PASS | No CORS changes needed for this feature |

**Status**: ✅ ALL GATES PASSED - Proceed to Phase 0

**Justification**: This feature extends the existing Goals table with a new color column and updates existing request/response models. No architectural changes or new dependencies required. All work stays within established MVC pattern using Dapper for data access.

### Post-Design Check (After Phase 1)

| Principle | Compliance | Design Verification |
|-----------|------------|---------------------|
| I. .NET 8 & Dapper ORM | ✅ PASS | Design uses Dapper for all data access. GoalRepository updated with parameterized queries for BgColor. No ORM changes. |
| II. SQLite Local Database | ✅ PASS | Migration adds TEXT column to Goals table. Schema remains SQLite-compatible. No new database dependencies. |
| III. MVC Folder Structure | ✅ PASS | Design follows Controllers → Services → Repositories flow. New ColorValidator in Helpers/ follows convention. |
| IV. Naming Conventions | ✅ PASS | GoalDBO.BgColor, CreateGoalRequest.BgColor, UpdateGoalRequest.BgColor follow [Property]Request/DBO patterns. |
| V. CORS Policy | ✅ PASS | No API structure changes affecting CORS. Existing CORS configuration sufficient. |

**Final Status**: ✅ ALL GATES PASSED - Design approved, proceed to implementation

**Design Compliance Notes**:
- ColorValidator helper class follows Helpers/ folder convention for utility functions
- Migration script (002_add_goal_bgcolor.sql) follows sequential numbering pattern
- All request/response models maintain nullable string (string?) for optional fields per .NET 8 conventions
- Error codes (INVALID_COLOR_FORMAT, INVALID_COLOR_RANGE) follow existing INVALID_REQUEST pattern
- Service layer validation maintains separation of concerns per MVC architecture

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

```text
VibeLoopBE/
├── Controllers/
│   └── GoalsController.cs          # MODIFY: Add color parameter handling
├── Services/
│   ├── IGoalService.cs             # MODIFY: Update method signatures for color
│   └── GoalService.cs              # MODIFY: Add color validation and normalization logic
├── Models/
│   ├── DBOs/
│   │   └── GoalDBO.cs              # MODIFY: Add BgColor property
│   ├── Requests/
│   │   ├── CreateGoalRequest.cs    # MODIFY: Add optional BgColor property
│   │   └── UpdateGoalRequest.cs    # MODIFY: Add optional BgColor property
│   └── Responses/
│       └── GoalResponse.cs         # MODIFY: Add BgColor property
├── Repositories/
│   ├── IGoalRepository.cs          # MODIFY: Update method signatures for color
│   └── GoalRepository.cs           # MODIFY: Update SQL queries to include BgColor
├── Helpers/                         # CREATE: New ColorValidator helper
│   └── ColorValidator.cs           # CREATE: RGB validation and normalization
├── Data/
│   ├── schema.sql                  # MODIFY: Add BgColor column to Goals table
│   └── migrations/                 # CREATE: Migration folder
│       └── 002_add_goal_bgcolor.sql # CREATE: Migration script
└── Filters/
    └── GlobalExceptionFilter.cs    # EXISTING: No changes needed
```

**Structure Decision**: Using existing .NET MVC Web API structure. This feature requires modifications to existing files across Controllers, Services, Models, and Repositories layers, plus a new ColorValidator helper for validation logic and a database migration script. No new architectural components needed - extending existing patterns.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

**No violations detected** - All constitution principles are followed. No complexity justification required.

## Phase Completion Summary

### Phase 0: Research ✅ COMPLETED

**Deliverable**: [research.md](./research.md)

**Research Areas Completed**:
1. ✅ RGB string storage in SQLite - Decision: TEXT column with rgb(R, G, B) format
2. ✅ RGB validation in C# - Decision: Custom Regex-based validator
3. ✅ Database migration strategy - Decision: ALTER TABLE with DEFAULT value
4. ✅ Service layer validation - Decision: Validate in Service, normalize before storage
5. ✅ Normalization strategy - Decision: Normalize during validation
6. ✅ Optional field handling - Decision: Nullable string (string?) in requests
7. ✅ Error response format - Decision: Existing ErrorResponse with new error codes

**Key Decisions**:
- No new dependencies required (using System.Text.RegularExpressions)
- Migration is safe and atomic with DEFAULT constraint
- Validation occurs in Service layer following existing patterns
- All colors normalized to "rgb(R, G, B)" format with single spaces

### Phase 1: Design & Contracts ✅ COMPLETED

**Deliverables**:
- [data-model.md](./data-model.md) - Complete entity definitions and schema changes
- [contracts/openapi.yaml](./contracts/openapi.yaml) - Full API contract specification
- [quickstart.md](./quickstart.md) - Usage examples and integration guide

**Design Artifacts**:
1. ✅ Data Model - GoalDBO with BgColor property, validation rules, state transitions
2. ✅ API Contracts - OpenAPI 3.0 spec with request/response examples and error cases
3. ✅ Migration Script - SQL for adding BgColor column with backfill
4. ✅ Request/Response Models - Updated CreateGoalRequest, UpdateGoalRequest, GoalResponse
5. ✅ Validation Logic - ColorValidator helper class design
6. ✅ Integration Examples - curl commands, JavaScript/React examples, error handling

**Constitution Re-Check**: ✅ PASSED - All design decisions comply with constitution principles

### Phase 2: Task Breakdown ⏭️ NEXT

**Command**: `/speckit.tasks`

**Expected Output**: `tasks.md` with implementation checklist organized by layer (Database → Models → Repositories → Services → Controllers → Testing)

**Note**: Phase 2 is NOT executed by `/speckit.plan`. Run `/speckit.tasks` separately to generate implementation tasks.

## Implementation Readiness

### ✅ Ready to Implement

All planning and design phases complete. No blockers or unresolved questions remain.

**Artifacts Available**:
- ✅ Feature specification with clarifications
- ✅ Technical research with all decisions documented
- ✅ Complete data model with migration script
- ✅ API contracts (OpenAPI spec)
- ✅ Usage examples and quickstart guide
- ✅ Constitution compliance verified twice

**Next Steps**:
1. Run `/speckit.tasks` to generate implementation task list
2. Execute tasks in order (Database → Models → Repositories → Services → Controllers)
3. Follow quickstart.md for testing each implementation step
4. Verify all acceptance scenarios from spec.md

### Files Modified Summary

**To be Created** (2 files):
- `VibeLoopBE/Helpers/ColorValidator.cs`
- `VibeLoopBE/Data/migrations/002_add_goal_bgcolor.sql`

**To be Modified** (9 files):
- `VibeLoopBE/Controllers/GoalsController.cs`
- `VibeLoopBE/Services/IGoalService.cs`
- `VibeLoopBE/Services/GoalService.cs`
- `VibeLoopBE/Models/DBOs/GoalDBO.cs`
- `VibeLoopBE/Models/Requests/CreateGoalRequest.cs`
- `VibeLoopBE/Models/Requests/UpdateGoalRequest.cs`
- `VibeLoopBE/Models/Responses/GoalResponse.cs`
- `VibeLoopBE/Repositories/IGoalRepository.cs`
- `VibeLoopBE/Repositories/GoalRepository.cs`

**Total Impact**: 11 files (2 new, 9 modified)

---

**Plan Status**: ✅ COMPLETE  
**Branch**: `002-goal-color`  
**Generated**: December 2, 2025  
**Command Used**: `/speckit.plan`
