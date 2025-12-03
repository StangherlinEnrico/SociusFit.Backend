namespace Domain.Entities;

public class Match
{
    public Guid Id { get; private set; }
    public Guid User1Id { get; private set; }
    public Guid User2Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Match() { }

    public Match(Guid user1Id, Guid user2Id)
    {
        Id = Guid.NewGuid();
        User1Id = user1Id;
        User2Id = user2Id;
        CreatedAt = DateTime.UtcNow;
    }

    public bool InvolvesBothUsers(Guid userId1, Guid userId2)
    {
        return (User1Id == userId1 && User2Id == userId2) ||
               (User1Id == userId2 && User2Id == userId1);
    }

    public Guid GetOtherUserId(Guid currentUserId)
    {
        return User1Id == currentUserId ? User2Id : User1Id;
    }
}