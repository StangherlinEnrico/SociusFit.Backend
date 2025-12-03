namespace Application.DTOs;

public class MatchDto
{
    public Guid MatchId { get; set; }
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public int OtherUserAge { get; set; }
    public string OtherUserCity { get; set; } = string.Empty;
    public string? OtherUserPhotoUrl { get; set; }
    public List<SportInfoDto> CommonSports { get; set; } = new();
    public DateTime MatchedAt { get; set; }
}