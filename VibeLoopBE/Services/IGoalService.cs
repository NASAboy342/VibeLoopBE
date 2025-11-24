using VibeLoopBE.Models.Requests;

namespace VibeLoopBE.Services;

public interface IGoalService
{
    Task<(bool Success, string? ErrorCode, string? ErrorMessage, string? GoalId)> CreateGoalAsync(CreateGoalRequest request);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateGoalAsync(UpdateGoalRequest request);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> DeleteGoalAsync(DeleteGoalRequest request);
}
