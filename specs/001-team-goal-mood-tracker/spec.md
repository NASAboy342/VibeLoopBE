# Feature Specification: Team Daily Goal Tracker with Mood Sync

**Feature Branch**: `001-team-goal-mood-tracker`  
**Created**: 2025-11-24  
**Status**: Draft  
**Input**: User description: "v1 Team Daily Goal Tracker with Mood Sync - Backend API for small teams to track daily goals and monitor team morale with mood tracking"

## Clarifications

### Session 2025-11-24

- Q: What structure should error responses follow when validation fails or resources aren't found? → A: Consistent error object with code and message (e.g., `{ "error": "VALIDATION_ERROR", "message": "Goal description must be..." }`)
- Q: How should the system handle simultaneous mood updates from the same team member (e.g., two devices updating mood at nearly the same time)? → A: Last-write-wins based on timestamp
- Q: Should the SQLite database file be committed to version control or gitignored with seed data script? → A: Gitignore database file, provide seed data script

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Team Daily Goals and Moods (Priority: P1)

A team manager opens the dashboard at the start of the day to see all team members, their current mood status, and their daily goals. This gives an instant snapshot of team well-being and workload distribution.

**Why this priority**: This is the core value proposition - providing visibility into team status. Without this, there's no dashboard to use. It's the foundation for all other features.

**Independent Test**: Can be fully tested by calling GET /api/members and verifying that all team members with their goals and mood status are returned. Delivers immediate value as a read-only team status dashboard.

**Acceptance Scenarios**:

1. **Given** the system has multiple team members with goals, **When** a manager requests the team overview, **Then** all team members are returned with their names, current mood, mood timestamp, and all their goals for any date
2. **Given** a team member has no mood set yet, **When** the overview is requested, **Then** that member's mood field is null with null timestamp
3. **Given** a team member has multiple goals from different dates, **When** the overview is requested, **Then** all goals are included regardless of date
4. **Given** the system is empty, **When** the overview is requested, **Then** an empty array is returned successfully

---

### User Story 2 - Track Individual Mood Status (Priority: P2)

A team member starts their day by logging their current mood (great, good, neutral, low, or stressed). Managers can see mood changes throughout the day to identify team members who might need support.

**Why this priority**: Mood tracking is the differentiator that helps managers proactively support struggling team members. It works independently of goals.

**Independent Test**: Can be fully tested by calling POST /api/members/mood to update mood and verifying the mood timestamp is updated. The GET endpoint will reflect the change. Delivers value even without goals.

**Acceptance Scenarios**:

1. **Given** a team member exists, **When** they update their mood to "stressed" with a timestamp, **Then** their mood is saved and the mood timestamp is recorded
2. **Given** a team member previously had mood "good", **When** they update to "low", **Then** the new mood replaces the old one with an updated timestamp
3. **Given** a team member updates their mood, **When** the team overview is fetched, **Then** the updated mood and timestamp are immediately visible
4. **Given** an invalid mood value is submitted, **When** the mood update is attempted, **Then** an error is returned with valid mood options

---

### User Story 3 - Create and Manage Daily Goals (Priority: P3)

Team members create goals at the beginning of their workday or during standup. Each goal is tagged with a date (typically today) and can be marked complete as work progresses.

**Why this priority**: Goals provide structure to the daily standup, but the feature still delivers value (mood tracking and visibility) without goal management.

**Independent Test**: Can be fully tested by calling POST /api/goals to create a goal, POST /api/goals/update to mark it complete, and verifying via GET /api/members. Works independently as a simple goal tracker.

**Acceptance Scenarios**:

1. **Given** a team member exists, **When** they create a goal with description "Complete API documentation" for today's date, **Then** the goal is created with completed=false and today's date
2. **Given** a goal exists with completed=false, **When** the member marks it as completed=true, **Then** the goal status updates and persists
3. **Given** a team member has 5 goals for today (3 completed, 2 pending), **When** the dashboard is viewed, **Then** completion percentage shows 60% for that member
4. **Given** a goal description is less than 3 characters, **When** goal creation is attempted, **Then** an error is returned requiring minimum 3 characters
5. **Given** a goal description exceeds 200 characters, **When** goal creation is attempted, **Then** an error is returned with maximum length constraint

---

### User Story 4 - Delete Goals (Priority: P4)

Team members can remove goals that were created by mistake or are no longer relevant. This keeps the goal list clean and accurate.

**Why this priority**: This is a nice-to-have feature for data cleanup. The system is fully functional without delete capability - users can simply ignore incorrect goals or mark them incomplete.

**Independent Test**: Can be fully tested by calling POST /api/goals/delete and verifying the goal no longer appears in GET /api/members response.

**Acceptance Scenarios**:

1. **Given** a goal exists, **When** a delete request is sent with the goal ID, **Then** the goal is permanently removed from the system
2. **Given** a goal has been deleted, **When** the team overview is fetched, **Then** the deleted goal does not appear in any member's goal list
3. **Given** an invalid or non-existent goal ID, **When** delete is attempted, **Then** an error is returned indicating the goal was not found

---

### Edge Cases

- What happens when a team member updates their mood multiple times in quick succession? (System uses last-write-wins: accepts all updates and keeps the one with the most recent timestamp value)
- What happens when someone tries to create a goal with an invalid date format? (System should return validation error specifying YYYY-MM-DD format)
- What happens when fetching team overview but database is empty? (Return empty array successfully, not an error)
- What happens when updating goal completion status for a non-existent goal ID? (Return 404 error indicating goal not found)
- What happens when mood timestamp is in the future? (Accept it - client controls timestamp, useful for testing and data backfilling)
- What happens when creating multiple goals with identical descriptions for the same member on the same day? (Allow it - system doesn't enforce uniqueness, user might legitimately have duplicate tasks)

## Requirements *(mandatory)*

### Functional Requirements

**API Endpoints:**

- **FR-001**: System MUST provide a GET endpoint at /api/members that returns all team members with their goals and mood status
- **FR-002**: System MUST provide a POST endpoint at /api/members/mood that accepts member ID, mood value, and timestamp to update mood
- **FR-003**: System MUST provide a POST endpoint at /api/goals that accepts member ID, description, and date to create a new goal
- **FR-004**: System MUST provide a POST endpoint at /api/goals/update that accepts goal ID and completion status to update a goal
- **FR-005**: System MUST provide a POST endpoint at /api/goals/delete that accepts goal ID to permanently remove a goal

**Data Validation:**

- **FR-006**: System MUST validate goal descriptions are between 3 and 200 characters inclusive
- **FR-007**: System MUST validate mood values are one of: 'great', 'good', 'neutral', 'low', 'stressed'
- **FR-008**: System MUST validate date format as YYYY-MM-DD for goal dates
- **FR-009**: System MUST validate that required fields (memberId, description, date, etc.) are present in requests

**Data Persistence:**

- **FR-010**: System MUST persist all team member data including name and current mood status
- **FR-011**: System MUST persist mood update timestamps in ISO 8601 format
- **FR-012**: System MUST persist all goals with their description, completion status, creation timestamp, and associated date
- **FR-013**: System MUST maintain relationships between goals and team members
- **FR-014**: System MUST allow team members to have multiple goals for the same or different dates

**Business Logic:**

- **FR-015**: System MUST allow mood to be null when a team member has not yet logged their mood
- **FR-016**: System MUST replace previous mood value when a team member updates their mood
- **FR-017**: System MUST use last-write-wins strategy for concurrent mood updates, accepting the update with the most recent timestamp value
- **FR-018**: System MUST set completed=false by default when creating new goals
- **FR-019**: System MUST record creation timestamp automatically when goals are created
- **FR-020**: System MUST return success=true when goal deletion completes successfully

**Response Format:**

- **FR-021**: System MUST return all timestamps in ISO 8601 format
- **FR-022**: System MUST return dates in YYYY-MM-DD format
- **FR-023**: System MUST return appropriate HTTP status codes (200 for success, 400 for validation errors, 404 for not found)
- **FR-024**: System MUST return error responses as JSON objects with two fields: "error" (error code string) and "message" (human-readable description). Example: `{ "error": "VALIDATION_ERROR", "message": "Goal description must be between 3 and 200 characters" }`
- **FR-025**: System MUST use consistent error codes across all endpoints: "VALIDATION_ERROR" (400), "NOT_FOUND" (404), "INVALID_REQUEST" (400)

**CORS and Frontend Integration:**

- **FR-026**: System MUST allow cross-origin requests from all domains to support the existing frontend application

### Key Entities

- **TeamMember**: Represents an individual on the team with a unique identifier, name, current mood status (nullable), and timestamp of last mood update (nullable). A member can have zero or more goals.

- **Goal**: Represents a daily task or objective with a unique identifier, description text (3-200 chars), completion status (boolean), creation timestamp, target date (YYYY-MM-DD), and relationship to a specific team member. Goals are independent entities that can be created, updated, and deleted.

- **Mood**: Represents the emotional/mental state of a team member at a point in time. Not stored as a separate entity but as attributes on TeamMember (current mood value and timestamp). Valid values are 'great', 'good', 'neutral', 'low', 'stressed', or null.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Frontend application successfully retrieves and displays all team members with their goals and moods in under 2 seconds
- **SC-002**: Team members can create a new goal and see it reflected in the dashboard within 1 second
- **SC-003**: Mood updates are saved and immediately visible to all team members viewing the dashboard
- **SC-004**: Goal completion status updates are reflected in real-time without requiring page refresh
- **SC-005**: System correctly calculates team completion percentage when goals are marked complete or incomplete
- **SC-006**: All API endpoints return appropriate error messages when validation fails (e.g., description too short, invalid mood)
- **SC-007**: System supports at least 50 team members with 10 goals each without performance degradation
- **SC-008**: Deleted goals are immediately removed from the dashboard view
- **SC-009**: API responses maintain consistent data structure matching the specified TypeScript interfaces
- **SC-010**: System handles simultaneous updates from multiple team members without data conflicts or loss

## Assumptions

- Team members are pre-populated in the system via seed data script (member management/registration is out of scope for v1)
- SQLite database file is gitignored and not committed to version control
- Seed data script provides test team members for local development
- No authentication or authorization required for v1 (all endpoints are publicly accessible)
- No historical mood tracking (only current mood is stored and displayed)
- Goals are not time-ordered within a member's goal list (frontend handles sorting if needed)
- Timezone handling is delegated to the client (backend stores UTC timestamps as provided)
- No goal assignment workflow (members create their own goals, no assignment from managers)
- No goal editing capability (only create, complete/uncomplete, and delete)
- No team-level aggregation of mood statistics (frontend calculates percentages and averages)
- Data retention is unlimited (no automatic cleanup of old goals)
- No pagination on member list (all members returned in single response, acceptable for small teams)
