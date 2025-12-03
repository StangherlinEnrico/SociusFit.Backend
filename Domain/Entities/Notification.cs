namespace Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Type { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string? Data { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsSent { get; private set; }

    private Notification() { }

    public Notification(Guid userId, string type, string title, string body, string? data = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Type = type;
        Title = title;
        Body = body;
        Data = data;
        CreatedAt = DateTime.UtcNow;
        IsSent = false;
    }

    public void MarkAsSent()
    {
        IsSent = true;
    }
}