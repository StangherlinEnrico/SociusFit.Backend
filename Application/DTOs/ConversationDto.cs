namespace Application.DTOs;

public class ConversationDto
{
    public Guid MatchId { get; set; }
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public string? OtherUserPhotoUrl { get; set; }
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}