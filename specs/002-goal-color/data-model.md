# Data Model: Goal Background Color

**Feature**: Goal Background Color | **Date**: December 2, 2025  
**Branch**: `002-goal-color`

## Overview

Data model changes for adding background color support to goals. This document defines the schema changes, entity relationships, and data validation rules.

## Entities

### Goal (Modified)

Represents a team member's daily goal with visual background color for organization.

**Properties**:

| Property | Type | Required | Constraints | Default | Description |
|----------|------|----------|-------------|---------|-------------|
| Id | string (GUID) | Yes | Primary Key | Auto-generated | Unique goal identifier |
| MemberId | string (GUID) | Yes | Foreign Key → TeamMembers.Id | - | Owner of the goal |
| Description | string | Yes | 3-200 characters | - | Goal description text |
| Completed | boolean | Yes | - | false | Completion status |
| CreatedAt | DateTime (ISO 8601) | Yes | - | Current timestamp | Creation timestamp |
| Date | string (YYYY-MM-DD) | Yes | Date format | - | Target date for goal |
| **BgColor** | **string** | **Yes** | **rgb(R, G, B) format, R/G/B: 0-255** | **'rgb(255, 255, 255)'** | **Background color** |

**Relationships**:
- `MemberId` → TeamMembers.Id (Many-to-One, CASCADE on delete)

**Indexes**:
- `idx_goals_memberid` on MemberId (existing)
- `idx_goals_date` on Date (existing)

### TeamMember (No Changes)

No modifications required to TeamMember entity.

## Database Schema Changes

### Migration 002: Add BgColor Column

**Change Type**: Schema Addition (Non-Breaking)

**SQL**:
```sql
-- Migration 002: Add BgColor to Goals
-- Date: 2025-12-02
-- Description: Add background color support to goals with default white color

BEGIN TRANSACTION;

-- Add BgColor column with default white color
-- This automatically populates existing rows with the default value
ALTER TABLE Goals 
ADD COLUMN BgColor TEXT NOT NULL DEFAULT 'rgb(255, 255, 255)';

-- Verification query (should return 0)
SELECT COUNT(*) AS GoalsWithInvalidColor 
FROM Goals 
WHERE BgColor IS NULL OR BgColor = '' OR LENGTH(BgColor) < 10;

COMMIT;
```

**Before**:
```sql
CREATE TABLE Goals (
    Id TEXT PRIMARY KEY NOT NULL,
    MemberId TEXT NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) >= 3 AND LENGTH(Description) <= 200),
    Completed INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL,
    Date TEXT NOT NULL,
    FOREIGN KEY (MemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);
```

**After**:
```sql
CREATE TABLE Goals (
    Id TEXT PRIMARY KEY NOT NULL,
    MemberId TEXT NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) >= 3 AND LENGTH(Description) <= 200),
    Completed INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL,
    Date TEXT NOT NULL,
    BgColor TEXT NOT NULL DEFAULT 'rgb(255, 255, 255)',
    FOREIGN KEY (MemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);
```

**Impact Analysis**:
- ✅ Non-breaking: Existing queries continue to work
- ✅ Backward compatible: DEFAULT value populates existing rows automatically
- ✅ No data loss: All existing goals receive default white color
- ✅ Rollback supported: Column can be dropped if needed (requires table recreation in SQLite)

## Data Validation Rules

### BgColor Validation

**Format Requirements**:
- Pattern: `rgb(R, G, B)` where R, G, B are integers
- R value: 0-255 inclusive
- G value: 0-255 inclusive
- B value: 0-255 inclusive
- Case: 'rgb' lowercase (normalized from input)
- Spacing: Single space after each comma (normalized from input)

**Valid Examples**:
- `rgb(255, 255, 255)` - White (default)
- `rgb(0, 0, 0)` - Black
- `rgb(255, 0, 0)` - Red
- `rgb(52, 152, 219)` - Blue
- `rgb(100, 100, 100)` - Gray

**Invalid Examples** (Rejected with HTTP 400):
- `red` - Named colors not supported
- `#FF0000` - Hex format not supported
- `255, 0, 0` - Missing rgb() wrapper
- `rgb(300, 0, 0)` - Value out of range (>255)
- `rgb(-10, 0, 0)` - Negative value
- `rgb(1.5, 2.3, 3.8)` - Decimal values not allowed
- `` (empty) - Empty string
- `   ` (whitespace) - Whitespace only

**Normalization Examples**:
- Input: `rgb(255,0,0)` → Stored: `rgb(255, 0, 0)`
- Input: `RGB(255, 0, 0)` → Stored: `rgb(255, 0, 0)`
- Input: `rgb( 255 , 0 , 0 )` → Stored: `rgb(255, 0, 0)`
- Input: `rgb(001, 002, 003)` → Stored: `rgb(1, 2, 3)`

## State Transitions

### Goal Creation with Color

```
[No Goal] 
    ↓ POST /api/goals (with BgColor)
[Goal Created] 
    → BgColor = provided value (normalized)
    → All other properties initialized

[No Goal] 
    ↓ POST /api/goals (without BgColor)
[Goal Created] 
    → BgColor = 'rgb(255, 255, 255)' (default)
    → All other properties initialized
```

### Goal Update with Color

```
[Existing Goal with Color A]
    ↓ POST /api/goals/update (with new BgColor B)
[Updated Goal]
    → BgColor = B (normalized)
    → Color A replaced

[Existing Goal with Color A]
    ↓ POST /api/goals/update (without BgColor)
[Updated Goal]
    → BgColor = A (unchanged)
    → Other properties may change
```

## Data Transfer Objects (DTOs)

### GoalDBO (Database Object)

```csharp
namespace VibeLoopBE.Models.DBOs;

public class GoalDBO
{
    public string Id { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Date { get; set; } = string.Empty;
    public string BgColor { get; set; } = "rgb(255, 255, 255)";  // NEW
}
```

### CreateGoalRequest

```csharp
namespace VibeLoopBE.Models.Requests;

public class CreateGoalRequest
{
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? BgColor { get; set; }  // NEW - Optional
}
```

### UpdateGoalRequest

```csharp
namespace VibeLoopBE.Models.Requests;

public class UpdateGoalRequest
{
    public string GoalId { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? Completed { get; set; }
    public string? BgColor { get; set; }  // NEW - Optional
}
```

### GoalResponse

```csharp
namespace VibeLoopBE.Models.Responses;

public class GoalResponse
{
    public string Id { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;  // NEW - Always included
}
```

## Data Integrity Rules

1. **Non-Null Guarantee**: Every goal MUST have a BgColor value (enforced by database DEFAULT constraint)
2. **Format Consistency**: All stored colors MUST be in normalized format `rgb(R, G, B)`
3. **Range Validity**: All RGB components MUST be 0-255 (enforced by application validation)
4. **Immutability on Read**: Color values never change during retrieval operations
5. **Validation Before Storage**: Invalid colors MUST be rejected before database write

## Migration Strategy

### For Existing Goals

**Approach**: Automatic backfill via DEFAULT constraint

**Process**:
1. Migration adds column with DEFAULT 'rgb(255, 255, 255)'
2. SQLite automatically populates existing rows with default
3. No manual UPDATE statement required
4. Verification query confirms all rows have valid color

**Result**: All existing goals immediately have white background color

### For New Goals

**Approach**: Application-level default with optional override

**Process**:
1. Controller receives CreateGoalRequest
2. Service checks if BgColor provided
3. If not provided → apply default 'rgb(255, 255, 255)'
4. If provided → validate and normalize
5. Repository stores validated color

## Performance Considerations

**Storage Impact**:
- Color string length: 15-21 characters (e.g., "rgb(255, 255, 255)")
- Storage overhead per goal: ~20 bytes
- Impact on 1000 goals: ~20 KB (negligible)

**Query Impact**:
- No impact on existing queries (column added at end)
- SELECT queries include color automatically
- No additional joins or subqueries needed

**Validation Impact**:
- Regex validation: <1ms per validation
- Normalization: <1ms per operation
- Total overhead per request: <2ms (well within 5s target)

## Backward Compatibility

**Database Level**:
- ✅ Existing queries work without modification
- ✅ Existing goals automatically get default color
- ✅ No breaking changes to schema structure

**API Level**:
- ✅ Existing create requests work (color defaults to white)
- ✅ Existing update requests work (color preserved)
- ✅ Response format extended (additive change)
- ⚠️ Clients must handle new BgColor field in responses

## Future Extensibility

**Potential Enhancements** (Out of scope for this feature):
- Color-based filtering: `GET /api/goals?color=rgb(255,0,0)`
- Color statistics: Most used colors per team
- Color presets: Predefined color palettes
- Color themes: User or team-level color schemes
- RGBA support: Transparency/opacity values
- Multiple color formats: Hex, HSL support

**Database Readiness**:
- Current TEXT storage can accommodate longer color strings
- No schema changes needed for format additions
- Index can be added if color-based queries become common
