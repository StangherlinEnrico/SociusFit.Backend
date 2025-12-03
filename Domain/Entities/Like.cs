namespace Domain.Entities;

public class Like
{
    public Guid Id { get; private set; }
    public Guid LikerUserId { get; private set; }
    public Guid LikedUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Like() { }

    public Like(Guid likerUserId, Guid likedUserId)
    {
        Id = Guid.NewGuid();
        LikerUserId = likerUserId;
        LikedUserId = likedUserId;
        CreatedAt = DateTime.UtcNow;
    }
}