namespace VibeLoopBE.Models.Requests;

public class CreateGoalRequest
{
    public string MemberId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}
