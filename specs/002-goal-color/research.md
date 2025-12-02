# Research: Goal Background Color

**Feature**: Goal Background Color | **Date**: December 2, 2025  
**Branch**: `002-goal-color`

## Overview

Research findings for implementing background color support in the Goals feature. This document resolves all technical decisions needed before design phase.

## Research Tasks

### 1. RGB String Storage in SQLite

**Question**: How to efficiently store and query RGB color strings in SQLite?

**Decision**: Store as TEXT column with format "rgb(R, G, B)"

**Rationale**:
- SQLite TEXT type is optimal for variable-length strings
- No performance impact for simple equality comparisons
- Human-readable in database inspection
- Direct compatibility with CSS rgb() function in frontend
- No need for parsing/conversion in most use cases

**Alternatives Considered**:
- INTEGER storage (packed RGB): More compact but requires bitwise operations, less readable
- Separate R/G/B columns: Unnecessary complexity, 3x storage overhead
- HEX format: Requires conversion, not as readable as rgb()

**Implementation Notes**:
- Column: `BgColor TEXT NOT NULL DEFAULT 'rgb(255, 255, 255)'`
- Default constraint ensures all goals have a color
- No CHECK constraint needed (validation in application layer)

### 2. RGB Validation Best Practices in C#

**Question**: What's the standard approach for validating RGB color strings in .NET?

**Decision**: Custom validator using Regex with value range checking

**Rationale**:
- System.Drawing.Color not available in .NET Core without additional dependencies
- Regex provides fast, reliable pattern matching for "rgb(R, G, B)" format
- Value validation (0-255) happens after successful pattern match
- Centralized validation logic in dedicated helper class

**Pattern**: `^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$`

**Alternatives Considered**:
- System.Drawing.Color: Not available in .NET Core by default, would require Windows Compatibility Pack
- FluentValidation library: Overkill for single validation rule
- Manual string parsing: Error-prone, less performant

**Implementation Notes**:
```csharp
public static class ColorValidator
{
    private static readonly Regex RgbPattern = new Regex(
        @"^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public static (bool IsValid, string? ErrorMessage, string? NormalizedColor) Validate(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return (false, "Color cannot be empty", null);

        var match = RgbPattern.Match(color.Trim());
        if (!match.Success)
            return (false, "Color must be in format 'rgb(R, G, B)' where R, G, B are numbers 0-255", null);

        int r = int.Parse(match.Groups[1].Value);
        int g = int.Parse(match.Groups[2].Value);
        int b = int.Parse(match.Groups[3].Value);

        if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            return (false, "RGB values must be between 0 and 255", null);

        string normalized = $"rgb({r}, {g}, {b})";
        return (true, null, normalized);
    }
}
```

### 3. Database Migration Strategy for Existing Goals

**Question**: How to safely add a non-nullable column with default value to existing Goals table?

**Decision**: Two-step migration: ALTER TABLE with DEFAULT, then verify

**Rationale**:
- SQLite supports ALTER TABLE ADD COLUMN with DEFAULT value
- DEFAULT constraint automatically populates existing rows
- Non-nullable constraint with DEFAULT is safe for existing data
- No separate UPDATE statement needed
- Atomic operation prevents inconsistent state

**Migration Script**:
```sql
-- Migration 002: Add BgColor to Goals
-- Date: 2025-12-02
-- Description: Add background color support to goals with default white color

BEGIN TRANSACTION;

-- Add BgColor column with default white color
ALTER TABLE Goals ADD COLUMN BgColor TEXT NOT NULL DEFAULT 'rgb(255, 255, 255)';

-- Verify all existing goals have the default color
-- This is a safety check, should return 0
SELECT COUNT(*) AS GoalsWithoutColor FROM Goals WHERE BgColor IS NULL OR BgColor = '';

-- Create index for potential future color-based queries (optional)
-- CREATE INDEX IF NOT EXISTS idx_goals_bgcolor ON Goals(BgColor);

COMMIT;
```

**Alternatives Considered**:
- Nullable column: Would require null handling in all code paths, inconsistent with requirement
- Separate UPDATE statement: Unnecessary since DEFAULT handles existing rows
- Two migrations (nullable first, then make non-nullable): Overly complex

**Rollback Plan**:
```sql
-- Rollback is not directly supported in SQLite for ALTER TABLE DROP COLUMN
-- Would require recreating table without the column
-- For development, drop and recreate database is acceptable
```

### 4. Service Layer Validation vs Controller Validation

**Question**: Where should color validation logic reside?

**Decision**: Service layer with early validation, controller handles HTTP mapping

**Rationale**:
- Service layer owns business logic including validation rules
- Controller remains thin, focused on HTTP concerns
- Service can be reused from other entry points (e.g., background jobs)
- Testability: Service validation can be unit tested independently
- Follows existing pattern in GoalService (member validation)

**Flow**:
1. Controller receives request with optional BgColor
2. Controller passes to Service
3. Service validates color format if provided
4. Service normalizes color or applies default
5. Service passes to Repository
6. Repository executes SQL with validated color

**Alternatives Considered**:
- Controller validation with attributes: Less flexible, harder to test, couples validation to HTTP
- Repository validation: Too late, violates single responsibility
- Separate validation service: Overkill for simple validation

### 5. Normalization Strategy

**Question**: When should color string normalization occur?

**Decision**: Normalize during validation in Service layer before storage

**Rationale**:
- Single normalization point prevents inconsistencies
- Storage layer receives pre-normalized data
- All database entries guaranteed consistent format
- Retrieval requires no normalization
- Simplifies frontend consumption

**Normalization Rules**:
- Remove extra whitespace: "rgb(255,0,0)" → "rgb(255, 0, 0)"
- Consistent spacing: single space after each comma
- Lowercase 'rgb': "RGB(255, 0, 0)" → "rgb(255, 0, 0)"
- Leading zeros removed: "rgb(001, 002, 003)" → "rgb(1, 2, 3)"

**Implementation**: Part of ColorValidator.Validate() method returns normalized string

### 6. Optional Field Handling in Requests

**Question**: How to differentiate between "field not provided" vs "field provided as null" vs "field provided with value"?

**Decision**: Use nullable string (string?) in Request models, apply default in Service if null or missing

**Rationale**:
- C# nullable reference types clearly express optionality
- Service layer can distinguish null from empty string
- Consistent with .NET 8 nullable annotations
- Simplifies controller code (no conditional checks)

**Implementation**:
```csharp
public class CreateGoalRequest
{
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? BgColor { get; set; }  // Nullable - optional field
}

// In Service:
string colorToUse = string.IsNullOrWhiteSpace(request.BgColor) 
    ? "rgb(255, 255, 255)"  // Default white
    : request.BgColor;      // Provided value (will be validated)
```

### 7. Error Response Format for Validation Failures

**Question**: What error format should be returned for invalid color values?

**Decision**: Use existing ErrorResponse model with specific error codes

**Rationale**:
- Consistent with existing error handling (INVALID_REQUEST, NOT_FOUND)
- GlobalExceptionFilter already handles structured errors
- Frontend can parse and display user-friendly messages
- Error code allows programmatic handling

**Error Codes**:
- `INVALID_COLOR_FORMAT`: When color string doesn't match rgb(R, G, B) pattern
- `INVALID_COLOR_RANGE`: When RGB values are outside 0-255 range
- `INVALID_REQUEST`: For other validation failures (existing code)

**Example Response**:
```json
{
  "error": "INVALID_COLOR_FORMAT",
  "message": "Color must be in format 'rgb(R, G, B)' where R, G, B are numbers 0-255"
}
```

## Summary of Decisions

| Area | Decision | Impact |
|------|----------|--------|
| Storage | TEXT column with rgb(R, G, B) format | Simple, readable, CSS-compatible |
| Validation | Regex-based validator in helper class | Fast, reliable, testable |
| Migration | ALTER TABLE with DEFAULT value | Safe, atomic, auto-populates existing rows |
| Validation Location | Service layer | Follows existing pattern, testable |
| Normalization | During validation before storage | Consistent data format |
| Optional Handling | Nullable string in requests | Clear optionality semantics |
| Error Format | Existing ErrorResponse with specific codes | Consistent API design |

## Technology Stack Confirmation

- ✅ .NET 8.0 Web API
- ✅ Dapper 2.1.28 (data access)
- ✅ SQLite with TEXT storage
- ✅ No new dependencies required
- ✅ System.Text.RegularExpressions (built-in)

## Dependencies & Integration Points

**Modified Components**:
- GoalsController: Add color parameter handling
- GoalService: Add validation and normalization
- GoalRepository: Update SQL queries
- Request/Response models: Add BgColor property
- GoalDBO: Add BgColor property

**New Components**:
- ColorValidator helper class
- Database migration script

**No Changes Required**:
- GlobalExceptionFilter (existing error handling sufficient)
- MembersController (unrelated)
- CORS configuration (no API changes affecting CORS)

## Risk Assessment

| Risk | Likelihood | Mitigation |
|------|-----------|------------|
| Invalid colors in production | Low | Validation at service layer prevents storage |
| Migration fails on existing data | Very Low | DEFAULT constraint handles existing rows automatically |
| Performance impact from validation | Very Low | Regex is fast, validation runs once per request |
| Frontend compatibility | Very Low | Standard CSS rgb() format |
| Concurrent updates losing color | Very Low | Existing transaction handling covers this |

## Next Steps

Proceed to Phase 1:
1. Create data-model.md with Goal entity including BgColor
2. Update API contracts in contracts/openapi.yaml
3. Generate quickstart.md with example requests
4. Re-verify Constitution Check with design decisions
