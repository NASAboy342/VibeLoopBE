# VibeLoop Backend - Team Daily Goal Tracker with Mood Sync

.NET 8 Web API backend for tracking daily team goals and monitoring team morale through mood tracking.

## Features

- 📊 **Team Dashboard**: View all team members with their goals and current mood status
- 😊 **Mood Tracking**: Team members can update their emotional status throughout the day
- ✅ **Goal Management**: Create, update, and complete daily goals
- 🗑️ **Goal Deletion**: Remove goals created by mistake

## Tech Stack

- **.NET 8 Web API** - Backend framework
- **Dapper** - Lightweight ORM for data access
- **SQLite** - Local database storage
- **Swagger/OpenAPI** - API documentation

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd VibeLoopBE
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run --project VibeLoopBE
   ```

   The API will start on `http://localhost:5133` (or check console output for actual port)

4. **Access Swagger UI**
   
   Navigate to: `http://localhost:5133/swagger`

### Database Setup

The SQLite database is automatically initialized on first run:
- Database file: `vibeloop.db` (gitignored)
- Schema: Applied from `Data/schema.sql`
- Seed data: Loaded from `Data/seed.sql` (8 test team members with sample goals)

## API Endpoints

### Get All Team Members
```bash
GET /api/members
```

Returns all team members with their goals and mood status.

**Example Response:**
```json
[
  {
    "id": "m1",
    "name": "Alice Johnson",
    "mood": "great",
    "moodUpdatedAt": "2025-11-24T08:00:00.000Z",
    "goals": [
      {
        "id": "g1",
        "memberId": "m1",
        "description": "Complete API documentation",
        "completed": true,
        "createdAt": "2025-11-24T08:05:00.000Z",
        "date": "2025-11-24"
      }
    ]
  }
]
```

### Update Member Mood
```bash
POST /api/members/mood
Content-Type: application/json

{
  "memberId": "m1",
  "mood": "great",
  "timestamp": "2025-11-24T10:00:00.000Z"
}
```

Valid mood values: `great`, `good`, `neutral`, `low`, `stressed`

### Create Goal
```bash
POST /api/goals
Content-Type: application/json

{
  "memberId": "m1",
  "description": "Write project documentation",
  "date": "2025-11-24"
}
```

### Update Goal Completion
```bash
POST /api/goals/update
Content-Type: application/json

{
  "goalId": "g1",
  "completed": true
}
```

### Delete Goal
```bash
POST /api/goals/delete
Content-Type: application/json

{
  "goalId": "g1"
}
```

## Project Structure

```
VibeLoopBE/
├── Controllers/           # API endpoints
│   ├── MembersController.cs
│   └── GoalsController.cs
├── Services/              # Business logic
│   ├── MemberService.cs
│   └── GoalService.cs
├── Repositories/          # Data access with Dapper
│   ├── MemberRepository.cs
│   └── GoalRepository.cs
├── Models/
│   ├── Requests/          # Request DTOs
│   ├── Responses/         # Response DTOs
│   └── DBOs/              # Database objects
├── Data/                  # Database scripts
│   ├── schema.sql
│   └── seed.sql
└── Filters/               # Exception handling
    └── GlobalExceptionFilter.cs
```

## Error Handling

All errors follow a consistent format:

```json
{
  "error": "ERROR_CODE",
  "message": "Human-readable description"
}
```

**Error Codes:**
- `INVALID_REQUEST` - Missing required fields
- `VALIDATION_ERROR` - Invalid data format or values
- `NOT_FOUND` - Resource doesn't exist
- `INTERNAL_ERROR` - Unexpected server error

## Development

### Build
```bash
dotnet build
```

### Run with hot reload
```bash
dotnet watch run --project VibeLoopBE
```

### Clean database (reset to seed data)
```bash
rm vibeloop.db
dotnet run --project VibeLoopBE
```

## Configuration

### CORS

CORS is configured to allow all origins for development. Update `Program.cs` to restrict origins in production:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Database Connection

Update connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=vibeloop.db"
  }
}
```

## Troubleshooting

### Port already in use
```bash
# Kill existing process
pkill -f "dotnet.*VibeLoopBE"

# Or specify different port in Properties/launchSettings.json
```

### Database locked
```bash
# Close any open SQLite connections
# Delete database file and restart
rm vibeloop.db
dotnet run --project VibeLoopBE
```

### CORS errors
Ensure CORS middleware is added before `UseAuthorization()` in `Program.cs`

## Documentation

Full feature specification and technical planning available in:
- `specs/001-team-goal-mood-tracker/spec.md` - Feature requirements
- `specs/001-team-goal-mood-tracker/plan.md` - Technical plan
- `specs/001-team-goal-mood-tracker/data-model.md` - Database schema
- `specs/001-team-goal-mood-tracker/contracts/` - OpenAPI specification

## License

MIT
