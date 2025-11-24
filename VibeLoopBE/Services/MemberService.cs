using VibeLoopBE.Models.Requests;
using VibeLoopBE.Models.Responses;
using VibeLoopBE.Repositories;

namespace VibeLoopBE.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private static readonly string[] ValidMoods = { "great", "good", "neutral", "low", "stressed" };

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<List<MemberResponse>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllMembersWithGoalsAsync();
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateMoodAsync(UpdateMoodRequest request)
    {
        // Validate mood value
        if (!ValidMoods.Contains(request.Mood.ToLower()))
        {
            return (false, "VALIDATION_ERROR", $"Mood must be one of: {string.Join(", ", ValidMoods)}");
        }

        // Check if member exists and get current timestamp
        var currentTimestamp = await _memberRepository.GetMemberMoodTimestampAsync(request.MemberId);
        if (currentTimestamp == null && !await MemberExistsAsync(request.MemberId))
        {
            return (false, "NOT_FOUND", $"Team member with ID '{request.MemberId}' not found");
        }

        // Last-write-wins logic: only update if incoming timestamp is newer
        if (currentTimestamp.HasValue && request.Timestamp <= currentTimestamp.Value)
        {
            // Silently accept but don't update (older timestamp)
            return (true, null, null);
        }

        // Update mood
        var updated = await _memberRepository.UpdateMemberMoodAsync(request.MemberId, request.Mood.ToLower(), request.Timestamp);
        
        return updated 
            ? (true, null, null) 
            : (false, "NOT_FOUND", $"Team member with ID '{request.MemberId}' not found");
    }

    private async Task<bool> MemberExistsAsync(string memberId)
    {
        var members = await _memberRepository.GetAllMembersWithGoalsAsync();
        return members.Any(m => m.Id == memberId);
    }
}
