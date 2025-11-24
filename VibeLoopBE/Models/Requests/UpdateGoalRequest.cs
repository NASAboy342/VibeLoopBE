namespace VibeLoopBE.Models.Requests;

public class UpdateGoalRequest
{
    public string GoalId { get; set; } = string.Empty;
    public bool Completed { get; set; }
}
