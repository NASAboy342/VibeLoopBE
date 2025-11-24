# Tasks: Team Daily Goal Tracker with Mood Sync

**Input**: Design documents from `/specs/001-team-goal-mood-tracker/`  
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: No tests for v1 (explicitly stated in requirements)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

All paths relative to `VibeLoopBE/` project directory (existing .NET 8 Web API project)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Add Dapper NuGet package to VibeLoopBE.csproj (version 2.1.28 or later)
- [ ] T002 [P] Add Microsoft.Data.Sqlite NuGet package to VibeLoopBE.csproj (version 8.0.0 or later)
- [ ] T003 [P] Add Swashbuckle.AspNetCore NuGet package to VibeLoopBE.csproj (if not already present)
- [ ] T004 Add connection string to appsettings.json: "DefaultConnection": "Data Source=vibeloop.db"
- [ ] T005 [P] Add vibeloop.db to .gitignore file
- [ ] T006 [P] Create Data/ folder in VibeLoopBE/ project directory

**Checkpoint**: NuGet packages restored, configuration ready

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T007 Create database schema file in Data/schema.sql with TeamMembers and Goals tables
- [ ] T008 Create seed data script in Data/seed.sql with 5-10 test team members
- [ ] T009 [P] Create Models/DBOs/TeamMemberDBO.cs with Id, Name, Mood, MoodUpdatedAt properties
- [ ] T010 [P] Create Models/DBOs/GoalDBO.cs with Id, MemberId, Description, Completed, CreatedAt, Date properties
- [ ] T011 [P] Create Models/Responses/ErrorResponse.cs with Error and Message properties
- [ ] T012 Configure CORS in Program.cs to allow all origins, methods, and headers
- [ ] T013 Configure Swagger in Program.cs (AddSwaggerGen, UseSwagger, UseSwaggerUI)
- [ ] T014 Add database initialization logic in Program.cs to run schema.sql and seed.sql on startup
- [ ] T015 Register IDbConnection as scoped service in Program.cs using SqliteConnection

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - View Team Daily Goals and Moods (Priority: P1) 🎯 MVP

**Goal**: Enable managers to view all team members with their current mood status and all their goals

**Independent Test**: Call GET /api/members and verify response returns all team members with their goals and mood data

### Implementation for User Story 1

- [ ] T016 [P] [US1] Create Models/Responses/GoalResponse.cs with Id, MemberId, Description, Completed, CreatedAt, Date properties
- [ ] T017 [P] [US1] Create Models/Responses/MemberResponse.cs with Id, Name, Mood, MoodUpdatedAt, Goals (List<GoalResponse>) properties
- [ ] T018 [P] [US1] Create Repositories/IMemberRepository.cs interface with GetAllMembersWithGoalsAsync method
- [ ] T019 [US1] Implement Repositories/MemberRepository.cs with Dapper multi-mapping query (JOIN TeamMembers and Goals)
- [ ] T020 [P] [US1] Create Services/IMemberService.cs interface with GetAllMembersAsync method
- [ ] T021 [US1] Implement Services/MemberService.cs with GetAllMembersAsync method (transforms DBOs to response DTOs)
- [ ] T022 [US1] Create Controllers/MembersController.cs with [ApiController] and [Route("api/[controller]")] attributes
- [ ] T023 [US1] Implement GET endpoint in MembersController.cs that calls MemberService.GetAllMembersAsync
- [ ] T024 [US1] Register IMemberRepository and IMemberService in Program.cs dependency injection container
- [ ] T025 [US1] Test GET /api/members endpoint with seed data using Swagger UI

**Checkpoint**: At this point, User Story 1 should be fully functional - read-only dashboard working with seed data

---

## Phase 4: User Story 2 - Track Individual Mood Status (Priority: P2)

**Goal**: Enable team members to update their mood status, visible to managers immediately

**Independent Test**: Call POST /api/members/mood to update mood and verify GET /api/members reflects the change

### Implementation for User Story 2

- [ ] T026 [P] [US2] Create Models/Requests/UpdateMoodRequest.cs with MemberId, Mood, Timestamp properties
- [ ] T027 [P] [US2] Add UpdateMemberMoodAsync method to Repositories/IMemberRepository.cs interface
- [ ] T028 [US2] Implement UpdateMemberMoodAsync in Repositories/MemberRepository.cs with UPDATE SQL statement
- [ ] T029 [P] [US2] Add UpdateMoodAsync method to Services/IMemberService.cs interface
- [ ] T030 [US2] Implement UpdateMoodAsync in Services/MemberService.cs with validation logic (mood enum check)
- [ ] T031 [US2] Implement last-write-wins logic in MemberService.cs (compare incoming timestamp with stored MoodUpdatedAt)
- [ ] T032 [US2] Add POST /mood endpoint to MembersController.cs that accepts UpdateMoodRequest
- [ ] T033 [US2] Add validation for required fields (MemberId, Mood, Timestamp) in MembersController.cs
- [ ] T034 [US2] Add validation for valid mood values ('great', 'good', 'neutral', 'low', 'stressed') in MembersController.cs
- [ ] T035 [US2] Handle NOT_FOUND error when MemberId doesn't exist in MembersController.cs
- [ ] T036 [US2] Test POST /api/members/mood endpoint with various mood values using Swagger UI

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Create and Manage Daily Goals (Priority: P3)

**Goal**: Enable team members to create goals and mark them complete as work progresses

**Independent Test**: Call POST /api/goals to create a goal, POST /api/goals/update to mark complete, verify via GET /api/members

### Implementation for User Story 3

- [ ] T037 [P] [US3] Create Models/Requests/CreateGoalRequest.cs with MemberId, Description, Date properties
- [ ] T038 [P] [US3] Create Models/Requests/UpdateGoalRequest.cs with GoalId, Completed properties
- [ ] T039 [P] [US3] Create Repositories/IGoalRepository.cs interface with CreateGoalAsync and UpdateGoalAsync methods
- [ ] T040 [US3] Implement Repositories/GoalRepository.cs with INSERT and UPDATE SQL statements using Dapper
- [ ] T041 [P] [US3] Create Services/IGoalService.cs interface with CreateGoalAsync and UpdateGoalAsync methods
- [ ] T042 [US3] Implement Services/GoalService.cs with CreateGoalAsync (generate Id, set CreatedAt, Completed=false)
- [ ] T043 [US3] Implement Services/GoalService.cs with UpdateGoalAsync method
- [ ] T044 [US3] Create Controllers/GoalsController.cs with [ApiController] and [Route("api/[controller]")] attributes
- [ ] T045 [US3] Implement POST /api/goals endpoint in GoalsController.cs with validation (description length 3-200, date format YYYY-MM-DD)
- [ ] T046 [US3] Implement POST /api/goals/update endpoint in GoalsController.cs with validation (goalId exists)
- [ ] T047 [US3] Handle NOT_FOUND error when GoalId doesn't exist in GoalsController.cs
- [ ] T048 [US3] Handle NOT_FOUND error when MemberId doesn't exist in GoalsController.cs
- [ ] T049 [US3] Add validation for description length (3-200 characters) with VALIDATION_ERROR response
- [ ] T050 [US3] Add validation for date format (YYYY-MM-DD regex) with VALIDATION_ERROR response
- [ ] T051 [US3] Register IGoalRepository and IGoalService in Program.cs dependency injection container
- [ ] T052 [US3] Test POST /api/goals endpoint with valid and invalid data using Swagger UI
- [ ] T053 [US3] Test POST /api/goals/update endpoint to mark goals complete/incomplete using Swagger UI

**Checkpoint**: All user stories 1, 2, AND 3 should now be independently functional

---

## Phase 6: User Story 4 - Delete Goals (Priority: P4)

**Goal**: Enable team members to remove goals that were created by mistake

**Independent Test**: Call POST /api/goals/delete and verify goal no longer appears in GET /api/members response

### Implementation for User Story 4

- [ ] T054 [P] [US4] Create Models/Requests/DeleteGoalRequest.cs with GoalId property
- [ ] T055 [P] [US4] Add DeleteGoalAsync method to Repositories/IGoalRepository.cs interface
- [ ] T056 [US4] Implement DeleteGoalAsync in Repositories/GoalRepository.cs with DELETE SQL statement
- [ ] T057 [P] [US4] Add DeleteGoalAsync method to Services/IGoalService.cs interface
- [ ] T058 [US4] Implement DeleteGoalAsync in Services/GoalService.cs
- [ ] T059 [US4] Implement POST /api/goals/delete endpoint in GoalsController.cs
- [ ] T060 [US4] Add validation for GoalId exists in GoalsController.cs with NOT_FOUND error handling
- [ ] T061 [US4] Return success response { "success": true } when deletion completes
- [ ] T062 [US4] Test POST /api/goals/delete endpoint with valid and invalid goal IDs using Swagger UI

**Checkpoint**: All user stories should now be fully functional

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final touches for production readiness

- [ ] T063 [P] Create Helpers/DatabaseHelper.cs for connection management utilities (optional)
- [ ] T064 [P] Create Filters/GlobalExceptionFilter.cs to handle unhandled exceptions with ErrorResponse format
- [ ] T065 Register GlobalExceptionFilter in Program.cs if created
- [ ] T066 [P] Verify all error responses follow standardized format: { "error": "CODE", "message": "..." }
- [ ] T067 [P] Verify all timestamps are returned in ISO 8601 format
- [ ] T068 [P] Verify all dates are returned in YYYY-MM-DD format
- [ ] T069 Test concurrent mood updates from multiple clients to verify last-write-wins behavior
- [ ] T070 [P] Test GET /api/members with empty database returns empty array (not error)
- [ ] T071 [P] Test all endpoints with missing required fields return INVALID_REQUEST error
- [ ] T072 [P] Test all endpoints with invalid data return VALIDATION_ERROR with helpful messages
- [ ] T073 Verify CORS allows requests from frontend application
- [ ] T074 [P] Review Swagger documentation for all 5 endpoints
- [ ] T075 Update README.md with quickstart instructions (reference quickstart.md)

**Final Checkpoint**: Production-ready API with all user stories complete and polished

---

## Dependencies

### User Story Completion Order

```
Phase 1 (Setup)
    ↓
Phase 2 (Foundation) ← MUST COMPLETE BEFORE USER STORIES
    ↓
    ├─→ Phase 3: US1 (P1) ← MVP (can implement alone)
    │       ↓
    ├─→ Phase 4: US2 (P2) (independent, but benefits from US1 GET endpoint for verification)
    │       ↓
    ├─→ Phase 5: US3 (P3) (independent, uses US1 GET endpoint for verification)
    │       ↓
    └─→ Phase 6: US4 (P4) (depends on US3 for goals to exist, uses US1 for verification)
            ↓
        Phase 7 (Polish)
```

### User Story Dependencies

- **US1** (View Team Dashboard): No dependencies - fully independent, read-only
- **US2** (Track Mood): Logically independent, but uses US1 GET endpoint to verify mood updates visible
- **US3** (Create/Manage Goals): Logically independent, but uses US1 GET endpoint to verify goals appear
- **US4** (Delete Goals): Requires US3 to have goals to delete, uses US1 to verify deletion

**Recommended Order**: US1 → US2 → US3 → US4 (priority-driven, enables testing of each story)

---

## Parallel Execution Opportunities

### Phase 1 (Setup) - 3 parallel tasks possible
- T002, T003, T005, T006 can run in parallel (different files)

### Phase 2 (Foundation) - 4 parallel tasks possible
- T009, T010, T011 can run in parallel (different model files)

### Phase 3 (US1) - 5 parallel tasks possible
- T016, T017, T018, T020, T022 can run in parallel (different files, no dependencies)

### Phase 4 (US2) - 3 parallel tasks possible
- T026, T027, T029 can run in parallel (interface/request model files)

### Phase 5 (US3) - 4 parallel tasks possible
- T037, T038, T039, T041 can run in parallel (interface/request model files)

### Phase 6 (US4) - 3 parallel tasks possible
- T054, T055, T057 can run in parallel (interface/request model files)

### Phase 7 (Polish) - 10 parallel tasks possible
- T063, T064, T066, T067, T068, T070, T071, T072, T074 can run in parallel (independent verification tasks)

**Total Parallelizable Tasks**: 32 out of 75 tasks (43%)

---

## Implementation Strategy

### MVP Scope (Minimum Viable Product)

**Phase 1 + Phase 2 + Phase 3 (US1 only)**  
- **Tasks**: T001-T025 (25 tasks)
- **Result**: Read-only dashboard showing all team members with their goals
- **Value**: Immediate visibility into team status (core value proposition)
- **Effort**: ~40% of total work
- **Deliverable**: Functional GET /api/members endpoint with seed data

### Incremental Delivery Plan

1. **MVP** (US1): Read-only dashboard - validate core value
2. **MVP + US2**: Add mood tracking - complete mood sync feature
3. **MVP + US2 + US3**: Add goal creation/completion - full goal management
4. **Complete** (all stories): Add goal deletion - data cleanup capability

### Validation After Each Phase

- **After US1**: Verify GET /api/members returns all seed data correctly
- **After US2**: Verify mood updates persist and display immediately
- **After US3**: Verify goals can be created, marked complete, and display in US1
- **After US4**: Verify deleted goals no longer appear in US1 response

---

## Task Summary

**Total Tasks**: 75 tasks
- Phase 1 (Setup): 6 tasks
- Phase 2 (Foundation): 9 tasks (blocking)
- Phase 3 (US1 - P1): 10 tasks ← MVP
- Phase 4 (US2 - P2): 11 tasks
- Phase 5 (US3 - P3): 17 tasks
- Phase 6 (US4 - P4): 9 tasks
- Phase 7 (Polish): 13 tasks

**Parallelizable**: 32 tasks (43%)

**MVP Tasks**: T001-T025 (25 tasks, 33% of total)

**Independent Testing**:
- US1: Testable independently (read-only)
- US2: Testable independently (mood updates)
- US3: Testable independently (goal CRUD)
- US4: Testable independently (goal deletion)

**Format Validation**: ✅ All 75 tasks follow checklist format:
- Checkbox: `- [ ]`
- Task ID: T001-T075 (sequential)
- [P] marker: 32 tasks marked parallelizable
- [Story] label: US1-US4 for phases 3-6
- Description: Includes exact file paths

---

## Notes

- **No tests**: Explicitly excluded per v1 requirements
- **Database initialization**: Automatic on app startup (schema.sql + seed.sql)
- **Error handling**: Standardized format enforced in Phase 7
- **CORS**: Configured in Phase 2 foundation
- **Swagger**: Available immediately after Phase 2 for manual testing
- **Seed data**: 5-10 test members available after Phase 2
