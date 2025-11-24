using VibeLoopBE.Models.Responses;

namespace VibeLoopBE.Repositories;

public interface IMemberRepository
{
    Task<List<MemberResponse>> GetAllMembersWithGoalsAsync();
    Task<bool> UpdateMemberMoodAsync(string memberId, string mood, DateTime timestamp);
    Task<DateTime?> GetMemberMoodTimestampAsync(string memberId);
}
