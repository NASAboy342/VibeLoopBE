using System.Data;
using Dapper;

namespace VibeLoopBE.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly IDbConnection _connection;

    public GoalRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<string> CreateGoalAsync(string memberId, string description, string date)
    {
        var goalId = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow;

        var sql = @"
            INSERT INTO Goals (Id, MemberId, Description, Completed, CreatedAt, Date)
            VALUES (@Id, @MemberId, @Description, 0, @CreatedAt, @Date)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = goalId,
            MemberId = memberId,
            Description = description,
            CreatedAt = createdAt,
            Date = date
        });

        return goalId;
    }

    public async Task<bool> UpdateGoalAsync(string goalId, bool completed)
    {
        var sql = @"
            UPDATE Goals 
            SET Completed = @Completed 
            WHERE Id = @GoalId";

        var rowsAffected = await _connection.ExecuteAsync(sql, new
        {
            GoalId = goalId,
            Completed = completed ? 1 : 0
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteGoalAsync(string goalId)
    {
        var sql = "DELETE FROM Goals WHERE Id = @GoalId";
        var rowsAffected = await _connection.ExecuteAsync(sql, new { GoalId = goalId });
        return rowsAffected > 0;
    }
}
