# Quickstart: Goal Background Color

**Feature**: Goal Background Color | **Branch**: `002-goal-color`

## Overview

This quickstart guide demonstrates how to use the new background color feature for goals. All examples use curl commands that can be run against the local development server.

## Prerequisites

- VibeLoopBE server running on `http://localhost:5000`
- At least one team member exists in the database
- curl command-line tool installed

## Base URL

```
http://localhost:5000/api
```

## Common Setup

For these examples, we'll assume a team member with ID `550e8400-e29b-41d4-a716-446655440000` exists.

## Use Cases

### 1. Create Goal with Custom Color

Create a goal with a red background for high-priority items.

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Fix critical production bug",
    "date": "2025-12-03",
    "bgColor": "rgb(231, 76, 60)"
  }'
```

**Response** (Success):
```json
{
  "success": true,
  "goalId": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Frontend Usage**:
```javascript
// The returned goalId can be used to retrieve the goal later
// The stored color will be: "rgb(231, 76, 60)" (normalized)
```

---

### 2. Create Goal with Default White Color

Create a goal without specifying color - automatically gets white background.

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Review pull requests",
    "date": "2025-12-03"
  }'
```

**Response** (Success):
```json
{
  "success": true,
  "goalId": "770e8400-e29b-41d4-a716-446655440002"
}
```

**Note**: This goal will have `bgColor: "rgb(255, 255, 255)"` in the database.

---

### 3. Create Goal with Flexible Spacing

Colors with different spacing formats are automatically normalized.

**Request** (no spaces):
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Team standup meeting",
    "date": "2025-12-03",
    "bgColor": "rgb(52,152,219)"
  }'
```

**Stored Value**: `"rgb(52, 152, 219)"` (normalized with single spaces)

---

### 4. Update Goal Color

Change the background color of an existing goal.

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals/update \
  -H "Content-Type: application/json" \
  -d '{
    "goalId": "660e8400-e29b-41d4-a716-446655440001",
    "bgColor": "rgb(46, 204, 113)"
  }'
```

**Response** (Success):
```json
{
  "success": true
}
```

**Result**: Only the color is updated; description, completion status, and other fields remain unchanged.

---

### 5. Update Goal Without Changing Color

Update goal properties while preserving the existing color.

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals/update \
  -H "Content-Type: application/json" \
  -d '{
    "goalId": "660e8400-e29b-41d4-a716-446655440001",
    "description": "Fixed critical production bug",
    "completed": true
  }'
```

**Response** (Success):
```json
{
  "success": true
}
```

**Result**: Description and completion status updated, but color remains `"rgb(46, 204, 113)"` from previous example.

---

### 6. Color Categories Example

Create goals with different colors for visual organization.

**High Priority (Red)**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Deploy to production",
    "date": "2025-12-03",
    "bgColor": "rgb(231, 76, 60)"
  }'
```

**Medium Priority (Yellow)**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Update documentation",
    "date": "2025-12-03",
    "bgColor": "rgb(241, 196, 15)"
  }'
```

**Low Priority (Green)**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Refactor old code",
    "date": "2025-12-03",
    "bgColor": "rgb(46, 204, 113)"
  }'
```

---

## Error Handling

### Invalid Color Format

**Request** (hex color - not supported):
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Test goal",
    "date": "2025-12-03",
    "bgColor": "#FF0000"
  }'
```

**Response** (Error - HTTP 400):
```json
{
  "error": "INVALID_COLOR_FORMAT",
  "message": "Color must be in format 'rgb(R, G, B)' where R, G, B are numbers 0-255"
}
```

---

### RGB Value Out of Range

**Request** (value > 255):
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Test goal",
    "date": "2025-12-03",
    "bgColor": "rgb(300, 0, 0)"
  }'
```

**Response** (Error - HTTP 400):
```json
{
  "error": "INVALID_COLOR_RANGE",
  "message": "RGB values must be between 0 and 255"
}
```

---

### Empty Color String

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Test goal",
    "date": "2025-12-03",
    "bgColor": ""
  }'
```

**Response** (Error - HTTP 400):
```json
{
  "error": "INVALID_COLOR_FORMAT",
  "message": "Color cannot be empty"
}
```

---

### Named Colors Not Supported

**Request**:
```bash
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{
    "memberId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Test goal",
    "date": "2025-12-03",
    "bgColor": "red"
  }'
```

**Response** (Error - HTTP 400):
```json
{
  "error": "INVALID_COLOR_FORMAT",
  "message": "Color must be in format 'rgb(R, G, B)' where R, G, B are numbers 0-255"
}
```

---

## Testing Checklist

Use these examples to verify the implementation:

- [ ] Create goal with custom color → Returns success with goalId
- [ ] Create goal without color → Returns success, defaults to white
- [ ] Create goal with flexible spacing → Normalizes correctly
- [ ] Update goal color only → Preserves other fields
- [ ] Update goal without color field → Preserves existing color
- [ ] Invalid format (hex) → Returns 400 with INVALID_COLOR_FORMAT
- [ ] Out of range value → Returns 400 with INVALID_COLOR_RANGE
- [ ] Empty string → Returns 400 with error message
- [ ] Named color → Returns 400 with INVALID_COLOR_FORMAT
- [ ] Negative value → Returns 400 with INVALID_COLOR_RANGE

---

## Common Color Values

Here are some commonly used RGB colors for quick reference:

| Color | RGB Value | Use Case |
|-------|-----------|----------|
| White | `rgb(255, 255, 255)` | Default/neutral |
| Red | `rgb(231, 76, 60)` | High priority/urgent |
| Orange | `rgb(230, 126, 34)` | Medium-high priority |
| Yellow | `rgb(241, 196, 15)` | Medium priority |
| Green | `rgb(46, 204, 113)` | Low priority/completed |
| Blue | `rgb(52, 152, 219)` | Info/reference |
| Purple | `rgb(155, 89, 182)` | Special/review |
| Gray | `rgb(149, 165, 166)` | Archived/inactive |
| Black | `rgb(0, 0, 0)` | Critical |

---

## Frontend Integration Example

### JavaScript/React

```javascript
// Create goal with color
const createGoal = async (memberId, description, date, color = null) => {
  const response = await fetch('http://localhost:5000/api/goals', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      memberId,
      description,
      date,
      ...(color && { bgColor: color })  // Only include if color provided
    })
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }
  
  return await response.json();
};

// Usage
try {
  const result = await createGoal(
    '550e8400-e29b-41d4-a716-446655440000',
    'Complete API documentation',
    '2025-12-03',
    'rgb(52, 152, 219)'
  );
  console.log('Goal created:', result.goalId);
} catch (error) {
  console.error('Failed to create goal:', error.message);
}
```

### CSS Rendering

```javascript
// Render goal with background color
const GoalCard = ({ goal }) => (
  <div style={{ backgroundColor: goal.bgColor, padding: '10px' }}>
    <h3>{goal.description}</h3>
    <p>Due: {goal.date}</p>
    <p>Status: {goal.completed ? 'Completed' : 'Pending'}</p>
  </div>
);
```

---

## Database Migration

Before using this feature, run the migration to add the BgColor column:

```bash
# Location: VibeLoopBE/Data/migrations/002_add_goal_bgcolor.sql
# The migration will automatically backfill existing goals with white color
```

After migration, verify existing goals:

```sql
SELECT Id, Description, BgColor FROM Goals LIMIT 5;
```

All existing goals should show `rgb(255, 255, 255)`.

---

## Troubleshooting

### Issue: Color not saving

**Symptom**: Color parameter ignored, always defaults to white.

**Solution**: Verify the BgColor property is included in the request body and formatted correctly.

### Issue: "INVALID_COLOR_FORMAT" error with valid color

**Symptom**: Valid rgb() color rejected.

**Possible Causes**:
- Missing parentheses: `rgb 255, 0, 0` ❌ → `rgb(255, 0, 0)` ✅
- Wrong format: `(255, 0, 0)` ❌ → `rgb(255, 0, 0)` ✅
- Decimal values: `rgb(255.5, 0, 0)` ❌ → `rgb(255, 0, 0)` ✅

### Issue: Color looks different than expected

**Symptom**: Stored color doesn't match input.

**Explanation**: This is expected - the system normalizes colors to standard format with single spaces. Input `rgb(255,0,0)` becomes `rgb(255, 0, 0)`.

---

## Next Steps

- See [data-model.md](./data-model.md) for detailed schema information
- See [contracts/openapi.yaml](./contracts/openapi.yaml) for complete API specification
- See [plan.md](./plan.md) for implementation architecture
