namespace VibeLoopBE.Models.Responses;

public class GoalResponse
{
    public string Id { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Date { get; set; } = string.Empty;
}
