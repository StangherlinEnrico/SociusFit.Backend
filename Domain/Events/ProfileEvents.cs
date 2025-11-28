namespace Domain.Events;

public class ProfileCreatedEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid UserId { get; private set; }
    public int Age { get; private set; }
    public string Gender { get; private set; }
    public string City { get; private set; }

    public ProfileCreatedEvent(Guid profileId, Guid userId, int age, string gender, string city)
    {
        ProfileId = profileId;
        UserId = userId;
        Age = age;
        Gender = gender;
        City = city;
    }
}

public class ProfileUpdatedEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid UserId { get; private set; }

    public ProfileUpdatedEvent(Guid profileId, Guid userId)
    {
        ProfileId = profileId;
        UserId = userId;
    }
}

public class ProfileCompletedEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid UserId { get; private set; }

    public ProfileCompletedEvent(Guid profileId, Guid userId)
    {
        ProfileId = profileId;
        UserId = userId;
    }
}

public class ProfileDeletedEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid UserId { get; private set; }

    public ProfileDeletedEvent(Guid profileId, Guid userId)
    {
        ProfileId = profileId;
        UserId = userId;
    }
}

public class SportAddedToProfileEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid SportId { get; private set; }
    public string SportName { get; private set; }

    public SportAddedToProfileEvent(Guid profileId, Guid sportId, string sportName)
    {
        ProfileId = profileId;
        SportId = sportId;
        SportName = sportName;
    }
}

public class SportRemovedFromProfileEvent : DomainEvent
{
    public Guid ProfileId { get; private set; }
    public Guid SportId { get; private set; }

    public SportRemovedFromProfileEvent(Guid profileId, Guid sportId)
    {
        ProfileId = profileId;
        SportId = sportId;
    }
}