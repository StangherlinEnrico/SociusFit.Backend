namespace Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid MatchId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }
    public bool IsRead { get; private set; }

    private Message() { }

    public Message(Guid matchId, Guid senderId, string content)
    {
        Id = Guid.NewGuid();
        MatchId = matchId;
        SenderId = senderId;
        Content = content;
        SentAt = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}