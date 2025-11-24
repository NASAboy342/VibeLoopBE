# Quickstart: Team Daily Goal Tracker with Mood Sync

**Feature**: 001-team-goal-mood-tracker  
**Date**: 2025-11-24  
**Prerequisites**: .NET 8 SDK installed

## Overview

This guide walks you through setting up and running the Team Daily Goal Tracker API locally.

---

## Setup

### 1. Install Dependencies

The project uses NuGet packages that will be restored automatically. Key dependencies:

```xml
<PackageReference Include="Dapper" Version="2.1.28" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

Restore packages:
```bash
cd VibeLoopBE
dotnet restore
```

### 2. Initialize Database

The SQLite database will be created automatically on first run. Schema and seed data scripts are located in `Data/` folder.

**Option A: Automatic (on application startup)**
- Database file `vibeloop.db` is created automatically
- Schema is applied from `Data/schema.sql`
- Seed data is inserted from `Data/seed.sql`

**Option B: Manual**
```bash
# From VibeLoopBE project directory
sqlite3 vibeloop.db < Data/schema.sql
sqlite3 vibeloop.db < Data/seed.sql
```

### 3. Configuration

Update `appsettings.json` with database connection string (default should work):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=vibeloop.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## Running the API

### Start Development Server

```bash
cd VibeLoopBE
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### Access Swagger UI

Open your browser and navigate to:
```
https://localhost:5001/swagger
```

This provides an interactive API documentation and testing interface.

---

## Quick API Test

### 1. Get All Members (Empty on First Run)

**Request:**
```bash
curl http://localhost:5000/api/members
```

**Expected Response:**
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "Alice Johnson",
    "mood": null,
    "moodUpdatedAt": null,
    "goals": []
  },
  {
    "id": "b2c3d4e5-f6g7-8901-bcde-fg2345678901",
    "name": "Bob Smith",
    "mood": null,
    "moodUpdatedAt": null,
    "goals": []
  }
]
```

### 2. Update Member Mood

**Request:**
```bash
curl -X POST http://localhost:5000/api/members/mood \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "mood": "great",
    "timestamp": "2025-11-24T08:30:00.000Z"
  }'
```

**Expected Response:**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "name": "Alice Johnson",
  "mood": "great",
  "moodUpdatedAt": "2025-11-24T08:30:00.000Z",
  "goals": []
}
```

### 3. Create a Goal

**Request:**
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "description": "Complete API documentation",
    "date": "2025-11-24"
  }'
```

**Expected Response:**
```json
{
  "id": "g1h2i3j4-k5l6-7890-mnop-qr1234567890",
  "memberId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "description": "Complete API documentation",
  "completed": false,
  "createdAt": "2025-11-24T08:35:00.000Z",
  "date": "2025-11-24"
}
```

### 4. Mark Goal Complete

**Request:**
```bash
curl -X POST http://localhost:5000/api/goals/update \
  -H "Content-Type: application/json" \
  -d '{
    "goalId": "g1h2i3j4-k5l6-7890-mnop-qr1234567890",
    "completed": true
  }'
```

### 5. Delete a Goal

**Request:**
```bash
curl -X POST http://localhost:5000/api/goals/delete \
  -H "Content-Type: application/json" \
  -d '{
    "goalId": "g1h2i3j4-k5l6-7890-mnop-qr1234567890"
  }'
```

**Expected Response:**
```json
{
  "success": true
}
```

---

## Project Structure

```
VibeLoopBE/
├── Controllers/
│   ├── MembersController.cs    # GET /api/members, POST /api/members/mood
│   └── GoalsController.cs      # POST /api/goals, /update, /delete
├── Services/
│   ├── MemberService.cs        # Business logic for members
│   └── GoalService.cs          # Business logic for goals
├── Repositories/
│   ├── MemberRepository.cs     # Dapper queries for members
│   └── GoalRepository.cs       # Dapper queries for goals
├── Models/
│   ├── Requests/               # API request DTOs
│   ├── Responses/              # API response DTOs
│   └── DBOs/                   # Database objects
├── Data/
│   ├── schema.sql              # Database schema
│   └── seed.sql                # Seed data
├── vibeloop.db                 # SQLite database (gitignored)
├── Program.cs                  # App startup & DI configuration
└── appsettings.json            # Configuration
```

---

## Development Workflow

### 1. Add New Team Members (Manual)

Connect to database:
```bash
sqlite3 vibeloop.db
```

Insert member:
```sql
INSERT INTO TeamMembers (Id, Name, Mood, MoodUpdatedAt)
VALUES ('new-uuid-here', 'Carol Lee', NULL, NULL);
```

### 2. View Database Contents

```bash
sqlite3 vibeloop.db

-- List all members
SELECT * FROM TeamMembers;

-- List all goals
SELECT * FROM Goals;

-- Join members with goals
SELECT m.Name, g.Description, g.Completed, g.Date
FROM TeamMembers m
LEFT JOIN Goals g ON m.Id = g.MemberId
ORDER BY m.Name, g.Date;
```

### 3. Reset Database

```bash
# Delete database file
rm vibeloop.db

# Restart application to recreate
dotnet run
```

---

## Troubleshooting

### Issue: Port Already in Use

**Error:** `Failed to bind to address http://127.0.0.1:5000`

**Solution:** Change port in `Properties/launchSettings.json` or kill existing process:
```bash
# Find process
lsof -ti:5000

# Kill process
kill -9 <PID>
```

### Issue: Database Locked

**Error:** `SQLite Error 5: 'database is locked'`

**Solution:** Close any open sqlite3 connections:
```bash
# Check for open connections
lsof vibeloop.db

# Kill sqlite3 processes
killall sqlite3
```

### Issue: CORS Errors from Frontend

**Error:** `Access to fetch at 'http://localhost:5000/api/members' from origin 'http://localhost:3000' has been blocked`

**Solution:** Verify CORS is configured in `Program.cs`:
```csharp
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors();  // Must be before UseAuthorization
```

### Issue: Invalid Mood Value

**Error:** `VALIDATION_ERROR: Mood must be one of: 'great', 'good', 'neutral', 'low', 'stressed'`

**Solution:** Check request body uses exact lowercase strings. Valid values:
- `"great"`
- `"good"`
- `"neutral"`
- `"low"`
- `"stressed"`

---

## Next Steps

1. **Review API Contracts**: See `contracts/openapi.yaml` for full API specification
2. **Implement Tasks**: Follow `tasks.md` (generated by `/speckit.tasks`) for implementation steps
3. **Connect Frontend**: Update frontend API base URL to `http://localhost:5000`
4. **Test Concurrent Updates**: Verify last-write-wins behavior with simultaneous mood updates

---

## Additional Resources

- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [SQLite with .NET](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Swagger/OpenAPI](https://swagger.io/specification/)

---

## Support

For issues or questions about this feature:
1. Check `spec.md` for functional requirements
2. Review `plan.md` for technical context
3. See `data-model.md` for database schema details
