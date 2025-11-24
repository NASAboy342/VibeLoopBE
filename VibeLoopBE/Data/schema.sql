-- Team Daily Goal Tracker with Mood Sync Database Schema
-- SQLite Database Schema

-- TeamMembers Table
CREATE TABLE IF NOT EXISTS TeamMembers (
    Id TEXT PRIMARY KEY NOT NULL,
    Name TEXT NOT NULL,
    Mood TEXT CHECK(Mood IN ('great', 'good', 'neutral', 'low', 'stressed') OR Mood IS NULL),
    MoodUpdatedAt TEXT  -- ISO 8601 format: YYYY-MM-DDTHH:MM:SS.sssZ
);

-- Goals Table
CREATE TABLE IF NOT EXISTS Goals (
    Id TEXT PRIMARY KEY NOT NULL,
    MemberId TEXT NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) >= 3 AND LENGTH(Description) <= 200),
    Completed INTEGER NOT NULL DEFAULT 0,  -- SQLite boolean: 0=false, 1=true
    CreatedAt TEXT NOT NULL,  -- ISO 8601 format
    Date TEXT NOT NULL,  -- YYYY-MM-DD format
    FOREIGN KEY (MemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_goals_memberid ON Goals(MemberId);
CREATE INDEX IF NOT EXISTS idx_goals_date ON Goals(Date);
