using VibeLoopBE.Models.Requests;
using VibeLoopBE.Models.Responses;

namespace VibeLoopBE.Services;

public interface IMemberService
{
    Task<List<MemberResponse>> GetAllMembersAsync();
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateMoodAsync(UpdateMoodRequest request);
}
