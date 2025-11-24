using System.Data;
using Dapper;
using VibeLoopBE.Models.DBOs;
using VibeLoopBE.Models.Responses;

namespace VibeLoopBE.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IDbConnection _connection;

    public MemberRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<MemberResponse>> GetAllMembersWithGoalsAsync()
    {
        var sql = @"
            SELECT 
                tm.Id, tm.Name, tm.Mood, tm.MoodUpdatedAt,
                g.Id, g.MemberId, g.Description, g.Completed, g.CreatedAt, g.Date
            FROM TeamMembers tm
            LEFT JOIN Goals g ON tm.Id = g.MemberId
            ORDER BY tm.Name, g.Date DESC, g.CreatedAt DESC";

        var memberDict = new Dictionary<string, MemberResponse>();

        await _connection.QueryAsync<TeamMemberDBO, GoalDBO?, MemberResponse>(
            sql,
            (member, goal) =>
            {
                if (!memberDict.TryGetValue(member.Id, out var memberResponse))
                {
                    memberResponse = new MemberResponse
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Mood = member.Mood,
                        MoodUpdatedAt = member.MoodUpdatedAt,
                        Goals = new List<GoalResponse>()
                    };
                    memberDict.Add(member.Id, memberResponse);
                }

                if (goal != null)
                {
                    memberResponse.Goals.Add(new GoalResponse
                    {
                        Id = goal.Id,
                        MemberId = goal.MemberId,
                        Description = goal.Description,
                        Completed = goal.Completed,
                        CreatedAt = goal.CreatedAt,
                        Date = goal.Date
                    });
                }

                return memberResponse;
            },
            splitOn: "Id"
        );

        return memberDict.Values.ToList();
    }

    public async Task<bool> UpdateMemberMoodAsync(string memberId, string mood, DateTime timestamp)
    {
        var sql = @"
            UPDATE TeamMembers 
            SET Mood = @Mood, MoodUpdatedAt = @Timestamp 
            WHERE Id = @MemberId";

        var rowsAffected = await _connection.ExecuteAsync(sql, new { MemberId = memberId, Mood = mood, Timestamp = timestamp });
        return rowsAffected > 0;
    }

    public async Task<DateTime?> GetMemberMoodTimestampAsync(string memberId)
    {
        var sql = "SELECT MoodUpdatedAt FROM TeamMembers WHERE Id = @MemberId";
        return await _connection.QueryFirstOrDefaultAsync<DateTime?>(sql, new { MemberId = memberId });
    }
}
