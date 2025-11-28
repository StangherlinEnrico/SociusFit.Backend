namespace Domain.Exceptions;

public class SportNotFoundException : DomainException
{
    public SportNotFoundException(Guid sportId)
        : base($"Sport not found with ID: {sportId}")
    {
    }

    public SportNotFoundException(string sportName)
        : base($"Sport not found with name: {sportName}")
    {
    }
}

public class InvalidSportDataException : DomainException
{
    public InvalidSportDataException(string message)
        : base(message)
    {
    }
}

public class TooManySportsException : DomainException
{
    public TooManySportsException(int maxAllowed)
        : base($"Cannot add more than {maxAllowed} sports to profile")
    {
    }
}

public class InsufficientSportsException : DomainException
{
    public InsufficientSportsException(int minRequired)
        : base($"Profile must have at least {minRequired} sport(s)")
    {
    }
}