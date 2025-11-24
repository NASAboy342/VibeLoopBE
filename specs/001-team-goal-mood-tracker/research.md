# Research: Team Daily Goal Tracker with Mood Sync

**Date**: 2025-11-24  
**Feature**: 001-team-goal-mood-tracker  
**Status**: Complete - No unknowns to resolve

## Overview

All technical decisions for this feature were pre-determined by the project constitution and clarified during the specification phase. No additional research required.

## Technology Decisions

### Decision: .NET 8 Web API with Dapper
**Rationale**: Mandated by project constitution (Principle I). Dapper provides lightweight, high-performance data access with direct SQL control.

**Alternatives considered**: Entity Framework (rejected per constitution - prohibited)

**Implementation notes**:
- Use `Microsoft.Data.Sqlite` NuGet package for SQLite connectivity
- Dapper methods: `Query<T>`, `QueryFirstOrDefault<T>`, `Execute` for CRUD operations
- Connection string in `appsettings.json`

---

### Decision: SQLite Local Database
**Rationale**: Constitution Principle II mandates SQLite. Provides zero-configuration embedded database suitable for small teams.

**Alternatives considered**: None (constitution requirement)

**Implementation notes**:
- Database file: `vibeloop.db` (gitignored per clarification)
- Schema migrations via `schema.sql` script
- Seed data via `seed.sql` script for test members

---

### Decision: MVC Folder Structure with Repository Pattern
**Rationale**: Constitution Principle III requires MVC organization. Repository pattern abstracts data access for testability.

**Alternatives considered**: Direct Dapper calls in services (rejected - violates separation of concerns)

**Implementation notes**:
- Controllers → Services → Repositories → Database
- Interface-based dependency injection
- Repositories handle all SQL and Dapper interactions

---

### Decision: Standardized Error Response Format
**Rationale**: Clarified during specification (Option B). Provides consistent error handling for frontend integration.

**Format**:
```json
{
  "error": "ERROR_CODE",
  "message": "Human-readable description"
}
```

**Error codes**: 
- `VALIDATION_ERROR` (400)
- `NOT_FOUND` (404)
- `INVALID_REQUEST` (400)

---

### Decision: Last-Write-Wins for Concurrent Mood Updates
**Rationale**: Clarified during specification. Simplest approach for v1, avoids complex locking.

**Implementation notes**:
- Compare incoming timestamp with stored `MoodUpdatedAt`
- Accept update with most recent timestamp value
- No explicit locking or transaction isolation required

---

### Decision: Seed Data Script Approach
**Rationale**: Clarified during specification (Option B). Standard practice for .NET projects with SQLite.

**Implementation notes**:
- `seed.sql` creates test team members
- Run on first application startup or manually
- Database file not committed to version control

---

## Best Practices Research

### Dapper with SQLite in .NET 8
- **Connection Management**: Use `IDbConnection` with dependency injection, single connection per request
- **Parameter Binding**: Always use parameterized queries: `@MemberId`, `@Description`
- **Transactions**: Use `IDbTransaction` for multi-statement operations (e.g., delete goal + cascade)
- **Date Handling**: Store as TEXT in ISO 8601 format, map to C# `DateTime` with UTC

### CORS Configuration
```csharp
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Swagger/OpenAPI Setup
```csharp
builder.Services.AddSwaggerGen();
app.UseSwagger();
app.UseSwaggerUI();
```

---

## Performance Considerations

### Target: <2s for GET /api/members (50 members × 10 goals)
- **Strategy**: Single SQL query with JOIN to fetch members and goals together
- **Dapper feature**: Multi-mapping (`Query<TeamMember, Goal, TeamMember>`)
- **Expected result**: ~500 rows returned, negligible for SQLite in-memory processing

### Target: <1s for POST operations
- **Strategy**: Simple INSERT/UPDATE queries, no complex transactions
- **SQLite advantage**: Local file access, no network latency

---

## Open Questions (NONE)

All technical questions resolved through constitution and clarification phases.

---

## References

- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [SQLite with .NET](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [ASP.NET Core CORS](https://docs.microsoft.com/en-us/aspnet/core/security/cors)
