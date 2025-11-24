namespace VibeLoopBE.Models.Responses;

public class MemberResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Mood { get; set; }
    public DateTime? MoodUpdatedAt { get; set; }
    public List<GoalResponse> Goals { get; set; } = new();
}
