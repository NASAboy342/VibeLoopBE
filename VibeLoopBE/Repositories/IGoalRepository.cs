namespace VibeLoopBE.Repositories;

public interface IGoalRepository
{
    Task<string> CreateGoalAsync(string memberId, string description, string date);
    Task<bool> UpdateGoalAsync(string goalId, bool completed);
    Task<bool> DeleteGoalAsync(string goalId);
}
