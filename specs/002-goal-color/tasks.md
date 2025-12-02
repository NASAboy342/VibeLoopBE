# Tasks: Goal Background Color

**Feature Branch**: `002-goal-color`  
**Input**: Design documents from `/specs/002-goal-color/`  
**Prerequisites**: ✅ plan.md, ✅ spec.md, ✅ research.md, ✅ data-model.md, ✅ contracts/openapi.yaml

**Tests**: Tests are NOT explicitly requested in the feature specification, so test tasks are OMITTED from this implementation plan.

**Organization**: Tasks are organized by user story to enable independent implementation and testing of each story.

## Format: `- [ ] [ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Setup/Foundational phases have NO story label
- Include exact file paths in descriptions

## Path Conventions

All paths relative to `VibeLoopBE/` directory (project root).

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and database migration setup

- [ ] T001 Create migrations directory at VibeLoopBE/Data/migrations/
- [ ] T002 Verify Dapper and Microsoft.Data.Sqlite dependencies in VibeLoopBE.csproj

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T003 Create database migration script VibeLoopBE/Data/migrations/002_add_goal_bgcolor.sql
- [ ] T004 Execute migration to add BgColor column to Goals table (ALTER TABLE with DEFAULT)
- [ ] T005 Verify migration success - confirm all existing goals have 'rgb(255, 255, 255)' as BgColor
- [ ] T006 [P] Create ColorValidator helper class in VibeLoopBE/Helpers/ColorValidator.cs
- [ ] T007 [P] Update GoalDBO model to include BgColor property in VibeLoopBE/Models/DBOs/GoalDBO.cs

**Checkpoint**: Foundation ready - database schema updated, validation helper available, DBO model extended

---

## Phase 3: User Story 1 - Assign Color When Creating Goal (Priority: P1) 🎯 MVP

**Goal**: Users can assign a visual background color to their goals when creating them, with optional color parameter that defaults to white

**Independent Test**: Create a goal with custom color via POST /api/goals with bgColor field, verify color is stored and can be retrieved. Create goal without color, verify it defaults to white.

### Implementation for User Story 1

- [ ] T008 [P] [US1] Add optional BgColor property to CreateGoalRequest in VibeLoopBE/Models/Requests/CreateGoalRequest.cs
- [ ] T009 [P] [US1] Add BgColor property to GoalResponse in VibeLoopBE/Models/Responses/GoalResponse.cs
- [ ] T010 [US1] Update IGoalRepository.CreateGoalAsync signature to include bgColor parameter in VibeLoopBE/Repositories/IGoalRepository.cs
- [ ] T011 [US1] Update GoalRepository.CreateGoalAsync to include BgColor in INSERT SQL with parameterized query in VibeLoopBE/Repositories/GoalRepository.cs
- [ ] T012 [US1] Update IGoalService.CreateGoalAsync signature to handle bgColor in VibeLoopBE/Services/IGoalService.cs
- [ ] T013 [US1] Update GoalService.CreateGoalAsync to validate and normalize color using ColorValidator, apply default white if not provided in VibeLoopBE/Services/GoalService.cs
- [ ] T014 [US1] Update GoalsController.CreateGoal to accept BgColor from request and return appropriate error codes (INVALID_COLOR_FORMAT, INVALID_COLOR_RANGE) in VibeLoopBE/Controllers/GoalsController.cs

**Verification**: 
- ✅ POST /api/goals with bgColor="rgb(255, 0, 0)" → Returns goalId, color stored as "rgb(255, 0, 0)"
- ✅ POST /api/goals without bgColor → Returns goalId, color stored as "rgb(255, 255, 255)"
- ✅ POST /api/goals with bgColor="rgb(255,0,0)" → Normalizes to "rgb(255, 0, 0)"
- ✅ POST /api/goals with bgColor="red" → Returns HTTP 400 with INVALID_COLOR_FORMAT
- ✅ POST /api/goals with bgColor="rgb(300, 0, 0)" → Returns HTTP 400 with INVALID_COLOR_RANGE

**Checkpoint**: User Story 1 complete - Goals can be created with custom or default colors, all validation working

---

## Phase 4: User Story 2 - Update Goal Color (Priority: P2)

**Goal**: Users can change the background color of existing goals, with optional color parameter that preserves existing color if not provided

**Independent Test**: Update an existing goal's color via POST /api/goals/update with bgColor field, verify new color is stored. Update goal without bgColor, verify existing color is preserved.

### Implementation for User Story 2

- [ ] T015 [P] [US2] Add optional BgColor property to UpdateGoalRequest in VibeLoopBE/Models/Requests/UpdateGoalRequest.cs
- [ ] T016 [US2] Update IGoalRepository.UpdateGoalAsync signature to include optional bgColor parameter in VibeLoopBE/Repositories/IGoalRepository.cs
- [ ] T017 [US2] Update GoalRepository.UpdateGoalAsync to conditionally include BgColor in UPDATE SQL when provided in VibeLoopBE/Repositories/GoalRepository.cs
- [ ] T018 [US2] Update IGoalService.UpdateGoalAsync signature to handle optional bgColor in VibeLoopBE/Services/IGoalService.cs
- [ ] T019 [US2] Update GoalService.UpdateGoalAsync to validate and normalize color when provided, preserve existing color when null in VibeLoopBE/Services/GoalService.cs
- [ ] T020 [US2] Update GoalsController.UpdateGoal to accept optional BgColor from request and return appropriate error codes in VibeLoopBE/Controllers/GoalsController.cs

**Verification**:
- ✅ POST /api/goals/update with bgColor="rgb(0, 255, 0)" → Returns success, color updated to "rgb(0, 255, 0)"
- ✅ POST /api/goals/update without bgColor → Returns success, existing color unchanged
- ✅ POST /api/goals/update with bgColor="#FF0000" → Returns HTTP 400 with INVALID_COLOR_FORMAT
- ✅ POST /api/goals/update with bgColor="" → Returns HTTP 400 error
- ✅ POST /api/goals/update for non-existent goal → Returns HTTP 404 NOT_FOUND (existing behavior)

**Checkpoint**: User Story 2 complete - Goals can be updated with new colors or preserve existing colors

---

## Phase 5: User Story 3 - View Goals with Colors (Priority: P3)

**Goal**: All API responses that return goal data include the background color property in normalized format

**Independent Test**: Retrieve goals through any endpoint, verify all responses include bgColor field with normalized rgb(R, G, B) format.

### Implementation for User Story 3

- [ ] T021 [US3] Verify GoalRepository retrieval methods include BgColor in SELECT queries in VibeLoopBE/Repositories/GoalRepository.cs
- [ ] T022 [US3] Update all GoalRepository methods that return GoalDBO to map BgColor from database in VibeLoopBE/Repositories/GoalRepository.cs
- [ ] T023 [US3] Ensure all Service layer methods that return goal data include BgColor in response mapping in VibeLoopBE/Services/GoalService.cs
- [ ] T024 [US3] Verify Controller methods return GoalResponse or similar objects that include BgColor in VibeLoopBE/Controllers/GoalsController.cs

**Note**: Since the current API only has create/update endpoints (no GET endpoints), this story primarily ensures the response from create operations includes the color. Future GET endpoints will automatically inherit this behavior.

**Verification**:
- ✅ Response from POST /api/goals includes bgColor field (already verified in US1)
- ✅ Any future GET /api/goals endpoints return bgColor for all goals
- ✅ All colors in responses use normalized format "rgb(R, G, B)" with single spaces
- ✅ Existing goals migrated with default white show "rgb(255, 255, 255)" in responses

**Checkpoint**: User Story 3 complete - All goal data includes color information in consistent format

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final improvements, documentation, and comprehensive validation

- [ ] T025 [P] Update schema.sql to reflect new Goals table structure with BgColor column in VibeLoopBE/Data/schema.sql
- [ ] T026 [P] Update API documentation (Swagger) to show BgColor parameter in create/update endpoints
- [ ] T027 Test all scenarios from quickstart.md - create with color, create without color, update color, update without color
- [ ] T028 Test all edge cases from spec.md - invalid formats, out of range values, empty strings, flexible spacing
- [ ] T029 Verify performance - create/update 100 goals with colors, confirm <5 second response times
- [ ] T030 Code review - verify ColorValidator follows C# conventions, proper error handling, no hardcoded values
- [ ] T031 Verify constitution compliance - Dapper usage, MVC structure, naming conventions, nullable annotations

**Final Checkpoint**: All acceptance criteria from spec.md verified, all edge cases handled, performance targets met

---

## Dependencies & Execution Order

### Phase Dependencies

```
Phase 1 (Setup)
    ↓
Phase 2 (Foundational) ← CRITICAL BLOCKER
    ↓
Phase 3 (US1) → Phase 4 (US2) → Phase 5 (US3) → Phase 6 (Polish)
    ↓              ↓              ↓
    └──────────────┴──────────────┘
         Can work in parallel if desired
```

- **Setup (Phase 1)**: No dependencies - start immediately
- **Foundational (Phase 2)**: Depends on Setup - BLOCKS all user stories
  - Migration must complete before any code changes
  - ColorValidator and GoalDBO must exist before Service/Repository changes
- **User Story 1 (Phase 3)**: Depends on Foundational - No dependencies on other stories
- **User Story 2 (Phase 4)**: Depends on Foundational - No dependencies on US1 (but builds on same files)
- **User Story 3 (Phase 5)**: Depends on Foundational - Verifies US1 and US2 implementations
- **Polish (Phase 6)**: Depends on all user stories being complete

### Recommended Execution Order

**For MVP (Minimum Viable Product)**:
1. Phase 1 (Setup)
2. Phase 2 (Foundational)
3. Phase 3 (US1 - Create with Color) ← Stop here for MVP
4. Validate with quickstart.md examples
5. Deploy MVP

**For Full Feature**:
1. Phase 1 (Setup)
2. Phase 2 (Foundational)
3. Phase 3 (US1 - Create with Color)
4. Phase 4 (US2 - Update Color)
5. Phase 5 (US3 - View with Colors)
6. Phase 6 (Polish)

### Task Dependencies Within Phases

**Phase 2 (Foundational)**:
- T003 (migration script) before T004 (execute migration)
- T004 (execute migration) before T005 (verify migration)
- T006 (ColorValidator) and T007 (GoalDBO) can run in parallel [P]
- All Phase 2 tasks must complete before starting Phase 3

**Phase 3 (US1)**:
- T008 (CreateGoalRequest) and T009 (GoalResponse) can run in parallel [P]
- T010 (IGoalRepository) before T011 (GoalRepository implementation)
- T012 (IGoalService) before T013 (GoalService implementation)
- T013 (GoalService) depends on T006 (ColorValidator) from Phase 2
- T014 (Controller) is last, depends on T013 (Service) being complete

**Phase 4 (US2)**:
- T015 (UpdateGoalRequest) can start immediately [P]
- T016 (IGoalRepository) before T017 (GoalRepository implementation)
- T018 (IGoalService) before T019 (GoalService implementation)
- T020 (Controller) is last, depends on T019 (Service) being complete

**Phase 5 (US3)**:
- T021-T024 are verification tasks, run sequentially
- All depend on Phase 3 and Phase 4 being complete

**Phase 6 (Polish)**:
- T025 (schema.sql) and T026 (Swagger docs) can run in parallel [P]
- T027-T031 are validation tasks, run in sequence

### Parallel Opportunities

**Phase 2 Parallelization** (2 parallel tracks):
```
Track 1: T003 → T004 → T005 (Migration)
Track 2: T006, T007 in parallel (Code)
Wait for both tracks before Phase 3
```

**Phase 3 Parallelization** (layer-by-layer):
```
Layer 1: T008, T009 in parallel (Models)
    ↓
Layer 2: T010 → T011 (Repository)
    ↓
Layer 3: T012 → T013 (Service) - depends on T006
    ↓
Layer 4: T014 (Controller)
```

**Cross-Story Parallelization** (if team capacity allows):
```
After Phase 2 completes:
Developer A: Phase 3 (US1)
Developer B: Phase 4 (US2) - works on same files but different methods
Wait for both to complete before Phase 5
```

**Phase 6 Parallelization**:
```
Track 1: T025, T026 in parallel (Documentation)
Track 2: T027-T031 in sequence (Testing)
```

---

## Parallel Example: Efficient Execution

If you have 2 developers and want to complete this feature quickly:

### Week 1, Day 1-2: Foundation (Both developers collaborate)
```bash
Developer A:
- T001: Create migrations directory
- T003-T005: Migration creation, execution, verification

Developer B:
- T002: Verify dependencies
- T006: Create ColorValidator
- T007: Update GoalDBO

Checkpoint: Foundation complete
```

### Week 1, Day 3-4: Core Feature (Split work)
```bash
Developer A: User Story 1 (P1)
- T008-T014: Full create with color implementation

Developer B: User Story 2 (P2)  
- T015-T020: Full update with color implementation

Checkpoint: Create and Update both working
```

### Week 1, Day 5: Verification & Polish (Both developers)
```bash
Developer A:
- T021-T024: US3 verification
- T027-T029: Test scenarios

Developer B:
- T025-T026: Documentation
- T030-T031: Code review and compliance

Checkpoint: Feature complete and validated
```

**Total Time**: 5 days with 2 developers (or 10 days with 1 developer)

---

## Implementation Strategy

### MVP-First Approach

**MVP = User Story 1 Only**

The minimum viable product consists of:
- ✅ Database migration (Phase 2: T003-T005)
- ✅ ColorValidator helper (Phase 2: T006)
- ✅ GoalDBO update (Phase 2: T007)
- ✅ Create goal with color (Phase 3: T008-T014)

**Value Delivered**:
- Users can create goals with custom colors
- Default white color applied automatically
- Validation prevents invalid colors
- Foundation for future enhancements

**Tasks for MVP**: T001-T014 (14 tasks)
**Estimated Time**: 3-5 days with 1 developer

### Incremental Delivery

After MVP, deliver incrementally:

1. **Increment 1** (MVP): Create with color
   - Deploy and gather user feedback
   - Validate color choices with real users

2. **Increment 2** (US2): Update color
   - Users can now change colors on existing goals
   - Enables color scheme adjustments

3. **Increment 3** (US3 + Polish): Complete feature
   - Verify all responses include colors
   - Final testing and documentation
   - Performance validation

### Risk Mitigation

**Migration Risk** (T003-T005):
- Test migration on copy of production database first
- Verify all goals get default color
- Have rollback plan ready (though SQLite doesn't support DROP COLUMN)

**Validation Logic Risk** (T006):
- Write comprehensive unit tests for ColorValidator
- Test edge cases: empty, null, malformed, out of range
- Verify normalization works correctly

**Integration Risk** (T013, T019):
- Service layer is critical - handles validation and defaults
- Test both "color provided" and "color omitted" paths
- Verify error messages are user-friendly

---

## Task Completion Checklist

Use this checklist to track progress:

### Phase 1: Setup
- [ ] Migrations directory created
- [ ] Dependencies verified

### Phase 2: Foundational
- [ ] Migration script created
- [ ] Migration executed successfully
- [ ] Migration verified (all goals have white)
- [ ] ColorValidator created and tested
- [ ] GoalDBO updated with BgColor property

### Phase 3: User Story 1 (MVP)
- [ ] CreateGoalRequest has BgColor
- [ ] GoalResponse has BgColor
- [ ] Repository interface updated
- [ ] Repository implementation updated
- [ ] Service interface updated
- [ ] Service implementation with validation
- [ ] Controller updated with error handling
- [ ] Tested: Create with color
- [ ] Tested: Create without color (defaults to white)
- [ ] Tested: Invalid format rejected
- [ ] Tested: Out of range rejected
- [ ] Tested: Flexible spacing normalized

### Phase 4: User Story 2
- [ ] UpdateGoalRequest has BgColor
- [ ] Repository interface updated
- [ ] Repository implementation updated
- [ ] Service interface updated
- [ ] Service implementation with validation
- [ ] Controller updated with error handling
- [ ] Tested: Update with new color
- [ ] Tested: Update without color (preserves existing)
- [ ] Tested: Invalid format rejected

### Phase 5: User Story 3
- [ ] Repository SELECT includes BgColor
- [ ] Service returns BgColor in responses
- [ ] Controller responses include BgColor
- [ ] Verified all responses normalized

### Phase 6: Polish
- [ ] schema.sql updated
- [ ] Swagger docs updated
- [ ] All quickstart.md scenarios pass
- [ ] All edge cases tested
- [ ] Performance validated (<5s)
- [ ] Code review complete
- [ ] Constitution compliance verified

---

**Total Tasks**: 31 tasks
**MVP Tasks**: 14 tasks (T001-T014)
**Full Feature Tasks**: 31 tasks

**Status**: Ready for implementation
**Branch**: `002-goal-color`
**Generated**: December 2, 2025
**Command Used**: `/speckit.tasks`
