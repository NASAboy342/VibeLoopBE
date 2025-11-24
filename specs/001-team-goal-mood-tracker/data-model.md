# Data Model: Team Daily Goal Tracker with Mood Sync

**Date**: 2025-11-24  
**Feature**: 001-team-goal-mood-tracker  
**Source**: Extracted from spec.md Key Entities section

## Entities

### TeamMember

**Purpose**: Represents an individual on the team with current mood tracking.

**Attributes**:
- `Id` (string, PK): Unique identifier for the team member (GUID)
- `Name` (string, required): Display name of the team member
- `Mood` (string, nullable): Current mood state - one of: 'great', 'good', 'neutral', 'low', 'stressed', or null
- `MoodUpdatedAt` (datetime, nullable): ISO 8601 timestamp of last mood update, null if mood never set

**Relationships**:
- Has many `Goals` (1:N relationship)

**Validation Rules**:
- Name: Required, non-empty
- Mood: Must be one of the 5 valid values or null
- MoodUpdatedAt: Must be valid ISO 8601 format if present

**State Transitions**:
- Initial state: Mood = null, MoodUpdatedAt = null
- Mood update: Replace both Mood and MoodUpdatedAt atomically
- Concurrent updates: Last-write-wins based on timestamp value

---

### Goal

**Purpose**: Represents a daily task or objective for a team member.

**Attributes**:
- `Id` (string, PK): Unique identifier for the goal (GUID)
- `MemberId` (string, FK): Reference to owning TeamMember
- `Description` (string, required): Goal text (3-200 characters)
- `Completed` (boolean, required): Completion status, default false
- `CreatedAt` (datetime, required): ISO 8601 timestamp when goal was created
- `Date` (string, required): Target date in YYYY-MM-DD format

**Relationships**:
- Belongs to one `TeamMember` (N:1 relationship)

**Validation Rules**:
- MemberId: Must reference existing TeamMember
- Description: 3-200 characters inclusive
- Date: Must match YYYY-MM-DD format
- Completed: Boolean (true/false)
- CreatedAt: Auto-generated on creation, immutable

**Lifecycle**:
- Create: Set Completed=false, auto-generate Id and CreatedAt
- Update: Only Completed field can be modified
- Delete: Permanent removal, no soft delete
- No uniqueness constraint on Description (duplicates allowed)

---

## Database Schema (SQLite)

```sql
CREATE TABLE IF NOT EXISTS TeamMembers (
    Id TEXT PRIMARY KEY NOT NULL,
    Name TEXT NOT NULL,
    Mood TEXT CHECK(Mood IN ('great', 'good', 'neutral', 'low', 'stressed') OR Mood IS NULL),
    MoodUpdatedAt TEXT  -- ISO 8601 format: YYYY-MM-DDTHH:MM:SS.sssZ
);

CREATE TABLE IF NOT EXISTS Goals (
    Id TEXT PRIMARY KEY NOT NULL,
    MemberId TEXT NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) >= 3 AND LENGTH(Description) <= 200),
    Completed INTEGER NOT NULL DEFAULT 0,  -- SQLite boolean: 0=false, 1=true
    CreatedAt TEXT NOT NULL,  -- ISO 8601 format
    Date TEXT NOT NULL,  -- YYYY-MM-DD format
    FOREIGN KEY (MemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_goals_memberid ON Goals(MemberId);
CREATE INDEX IF NOT EXISTS idx_goals_date ON Goals(Date);
```

---

## C# Model Classes

### Database Objects (DBOs)

**TeamMemberDBO.cs** (Maps to TeamMembers table):
```csharp
public class TeamMemberDBO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Mood { get; set; }
    public DateTime? MoodUpdatedAt { get; set; }
}
```

**GoalDBO.cs** (Maps to Goals table):
```csharp
public class GoalDBO
{
    public string Id { get; set; }
    public string MemberId { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Date { get; set; }  // Stored as string in YYYY-MM-DD format
}
```

### Request Models

**UpdateMoodRequest.cs**:
```csharp
public class UpdateMoodRequest
{
    public string MemberId { get; set; }
    public string Mood { get; set; }  // 'great', 'good', 'neutral', 'low', 'stressed'
    public DateTime Timestamp { get; set; }  // ISO 8601
}
```

**CreateGoalRequest.cs**:
```csharp
public class CreateGoalRequest
{
    public string MemberId { get; set; }
    public string Description { get; set; }  // 3-200 chars
    public string Date { get; set; }  // YYYY-MM-DD
}
```

**UpdateGoalRequest.cs**:
```csharp
public class UpdateGoalRequest
{
    public string GoalId { get; set; }
    public bool Completed { get; set; }
}
```

**DeleteGoalRequest.cs**:
```csharp
public class DeleteGoalRequest
{
    public string GoalId { get; set; }
}
```

### Response Models

**GoalResponse.cs**:
```csharp
public class GoalResponse
{
    public string Id { get; set; }
    public string MemberId { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }  // ISO 8601
    public string Date { get; set; }  // YYYY-MM-DD
}
```

**MemberResponse.cs**:
```csharp
public class MemberResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Mood { get; set; }
    public DateTime? MoodUpdatedAt { get; set; }  // ISO 8601
    public List<GoalResponse> Goals { get; set; }
}
```

**ErrorResponse.cs**:
```csharp
public class ErrorResponse
{
    public string Error { get; set; }  // Error code: VALIDATION_ERROR, NOT_FOUND, INVALID_REQUEST
    public string Message { get; set; }  // Human-readable description
}
```

---

## Data Flow

### GET /api/members
1. Repository: Query TeamMembers with LEFT JOIN Goals
2. Dapper multi-mapping to aggregate goals per member
3. Service: Transform DBOs to MemberResponse DTOs
4. Controller: Return List<MemberResponse>

### POST /api/members/mood
1. Controller: Validate request (mood value, memberId exists)
2. Service: Apply last-write-wins logic (compare timestamps)
3. Repository: UPDATE TeamMembers SET Mood=?, MoodUpdatedAt=? WHERE Id=?
4. Controller: Return updated MemberResponse

### POST /api/goals
1. Controller: Validate request (description length, date format, memberId exists)
2. Service: Generate new Id (GUID), set CreatedAt (UTC now), Completed=false
3. Repository: INSERT INTO Goals
4. Controller: Return GoalResponse

### POST /api/goals/update
1. Controller: Validate request (goalId exists)
2. Service: No business logic (pass-through)
3. Repository: UPDATE Goals SET Completed=? WHERE Id=?
4. Controller: Return updated GoalResponse

### POST /api/goals/delete
1. Controller: Validate request (goalId exists)
2. Service: No business logic (pass-through)
3. Repository: DELETE FROM Goals WHERE Id=?
4. Controller: Return { success: true }

---

## Indexing Strategy

- **Primary Keys**: Automatic index on TeamMembers.Id and Goals.Id
- **Foreign Key**: Index on Goals.MemberId for efficient JOIN operations
- **Date Filtering**: Index on Goals.Date (optional for v1, useful if filtering by date becomes common)

**Rationale**: With max 50 members × 10 goals (500 rows), indexing is not critical for performance. Primary indexes on PKs/FKs sufficient for v1.

---

## Data Constraints Summary

| Constraint | Enforcement | Location |
|------------|-------------|----------|
| Mood values | CHECK constraint | Database + API validation |
| Description length (3-200) | CHECK constraint | Database + API validation |
| Required fields | NOT NULL | Database + API validation |
| Foreign key integrity | FOREIGN KEY | Database with CASCADE delete |
| Date format (YYYY-MM-DD) | API validation | API layer only |
| ISO 8601 timestamps | API validation | API layer only |
| Unique IDs | PRIMARY KEY | Database |

---

## Migration Notes

**Initial Migration** (001_CreateTables.sql):
- Create TeamMembers table
- Create Goals table with foreign key
- Create indexes
- Should be idempotent (IF NOT EXISTS)

**Seed Data** (seed.sql):
- INSERT test team members (5-10 members)
- Sample names: "Alice Johnson", "Bob Smith", "Carol Lee", etc.
- All with Mood=null, MoodUpdatedAt=null initially
- No seed goals (let users create them via API)
