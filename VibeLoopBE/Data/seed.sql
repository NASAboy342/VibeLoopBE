-- Seed data for Team Daily Goal Tracker with Mood Sync
-- Provides test team members and sample goals

-- Insert test team members
INSERT OR IGNORE INTO TeamMembers (Id, Name, Mood, MoodUpdatedAt) VALUES
('m1', 'Alice Johnson', 'great', '2025-11-24T08:00:00.000Z'),
('m2', 'Bob Smith', 'good', '2025-11-24T08:15:00.000Z'),
('m3', 'Charlie Davis', 'neutral', '2025-11-24T08:30:00.000Z'),
('m4', 'Diana Martinez', 'low', '2025-11-24T08:45:00.000Z'),
('m5', 'Ethan Wilson', 'stressed', '2025-11-24T09:00:00.000Z'),
('m6', 'Fiona Brown', NULL, NULL),
('m7', 'George Taylor', 'good', '2025-11-24T09:15:00.000Z'),
('m8', 'Hannah Lee', 'great', '2025-11-24T09:30:00.000Z');

-- Insert sample goals for today (2025-11-24)
INSERT OR IGNORE INTO Goals (Id, MemberId, Description, Completed, CreatedAt, Date) VALUES
-- Alice's goals (3 total, 2 completed)
('g1', 'm1', 'Complete API documentation', 1, '2025-11-24T08:05:00.000Z', '2025-11-24'),
('g2', 'm1', 'Review pull requests', 1, '2025-11-24T08:10:00.000Z', '2025-11-24'),
('g3', 'm1', 'Update project roadmap', 0, '2025-11-24T08:15:00.000Z', '2025-11-24'),

-- Bob's goals (2 total, 1 completed)
('g4', 'm2', 'Fix login bug', 1, '2025-11-24T08:20:00.000Z', '2025-11-24'),
('g5', 'm2', 'Implement user settings page', 0, '2025-11-24T08:25:00.000Z', '2025-11-24'),

-- Charlie's goals (4 total, 0 completed)
('g6', 'm3', 'Refactor database queries', 0, '2025-11-24T08:35:00.000Z', '2025-11-24'),
('g7', 'm3', 'Optimize image loading', 0, '2025-11-24T08:40:00.000Z', '2025-11-24'),
('g8', 'm3', 'Add error handling to API', 0, '2025-11-24T08:45:00.000Z', '2025-11-24'),
('g9', 'm3', 'Write unit tests for services', 0, '2025-11-24T08:50:00.000Z', '2025-11-24'),

-- Diana's goals (1 total, 0 completed)
('g10', 'm4', 'Research authentication providers', 0, '2025-11-24T08:50:00.000Z', '2025-11-24'),

-- Ethan's goals (5 total, 3 completed)
('g11', 'm5', 'Deploy to staging', 1, '2025-11-24T09:05:00.000Z', '2025-11-24'),
('g12', 'm5', 'Update deployment scripts', 1, '2025-11-24T09:10:00.000Z', '2025-11-24'),
('g13', 'm5', 'Monitor server performance', 0, '2025-11-24T09:15:00.000Z', '2025-11-24'),
('g14', 'm5', 'Backup database', 1, '2025-11-24T09:20:00.000Z', '2025-11-24'),
('g15', 'm5', 'Configure SSL certificates', 0, '2025-11-24T09:25:00.000Z', '2025-11-24'),

-- Fiona has no goals yet

-- George's goals (2 total, 2 completed)
('g16', 'm7', 'Team standup meeting', 1, '2025-11-24T09:20:00.000Z', '2025-11-24'),
('g17', 'm7', 'Plan sprint retrospective', 1, '2025-11-24T09:25:00.000Z', '2025-11-24'),

-- Hannah's goals (3 total, 1 completed)
('g18', 'm8', 'Design new landing page', 1, '2025-11-24T09:35:00.000Z', '2025-11-24'),
('g19', 'm8', 'Create user flow diagrams', 0, '2025-11-24T09:40:00.000Z', '2025-11-24'),
('g20', 'm8', 'Update style guide', 0, '2025-11-24T09:45:00.000Z', '2025-11-24');

-- Add some goals for yesterday (2025-11-23) to test multi-date functionality
INSERT OR IGNORE INTO Goals (Id, MemberId, Description, Completed, CreatedAt, Date) VALUES
('g21', 'm1', 'Yesterday completed task', 1, '2025-11-23T10:00:00.000Z', '2025-11-23'),
('g22', 'm2', 'Yesterday incomplete task', 0, '2025-11-23T10:30:00.000Z', '2025-11-23');
