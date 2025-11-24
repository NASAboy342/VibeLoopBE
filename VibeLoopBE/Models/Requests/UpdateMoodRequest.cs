namespace VibeLoopBE.Models.Requests;

public class UpdateMoodRequest
{
    public string MemberId { get; set; } = string.Empty;
    public string Mood { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
