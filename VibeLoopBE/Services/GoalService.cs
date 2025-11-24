using System.Text.RegularExpressions;
using VibeLoopBE.Models.Requests;
using VibeLoopBE.Repositories;

namespace VibeLoopBE.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;
    private readonly IMemberRepository _memberRepository;
    private static readonly Regex DateRegex = new(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);

    public GoalService(IGoalRepository goalRepository, IMemberRepository memberRepository)
    {
        _goalRepository = goalRepository;
        _memberRepository = memberRepository;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, string? GoalId)> CreateGoalAsync(CreateGoalRequest request)
    {
        // Validate description length
        if (request.Description.Length < 3 || request.Description.Length > 200)
        {
            return (false, "VALIDATION_ERROR", "Goal description must be between 3 and 200 characters", null);
        }

        // Validate date format
        if (!DateRegex.IsMatch(request.Date))
        {
            return (false, "VALIDATION_ERROR", "Date must be in YYYY-MM-DD format", null);
        }

        // Check if member exists
        var members = await _memberRepository.GetAllMembersWithGoalsAsync();
        if (!members.Any(m => m.Id == request.MemberId))
        {
            return (false, "NOT_FOUND", $"Team member with ID '{request.MemberId}' not found", null);
        }

        // Create goal
        var goalId = await _goalRepository.CreateGoalAsync(request.MemberId, request.Description, request.Date);
        
        return (true, null, null, goalId);
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateGoalAsync(UpdateGoalRequest request)
    {
        var updated = await _goalRepository.UpdateGoalAsync(request.GoalId, request.Completed);
        
        if (!updated)
        {
            return (false, "NOT_FOUND", $"Goal with ID '{request.GoalId}' not found");
        }

        return (true, null, null);
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> DeleteGoalAsync(DeleteGoalRequest request)
    {
        var deleted = await _goalRepository.DeleteGoalAsync(request.GoalId);
        
        if (!deleted)
        {
            return (false, "NOT_FOUND", $"Goal with ID '{request.GoalId}' not found");
        }

        return (true, null, null);
    }
}
