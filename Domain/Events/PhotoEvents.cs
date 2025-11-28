namespace Domain.Events;

public class PhotoUploadedEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public Guid ProfileId { get; private set; }
    public string PhotoUrl { get; private set; }

    public PhotoUploadedEvent(Guid userId, Guid profileId, string photoUrl)
    {
        UserId = userId;
        ProfileId = profileId;
        PhotoUrl = photoUrl;
    }
}

public class PhotoDeletedEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public Guid ProfileId { get; private set; }
    public string PhotoUrl { get; private set; }

    public PhotoDeletedEvent(Guid userId, Guid profileId, string photoUrl)
    {
        UserId = userId;
        ProfileId = profileId;
        PhotoUrl = photoUrl;
    }
}