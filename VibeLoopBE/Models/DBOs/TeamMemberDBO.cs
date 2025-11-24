namespace VibeLoopBE.Models.DBOs;

public class TeamMemberDBO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Mood { get; set; }
    public DateTime? MoodUpdatedAt { get; set; }
}
