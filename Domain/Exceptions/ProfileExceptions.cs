namespace Domain.Exceptions;

public class ProfileNotFoundException : DomainException
{
    public ProfileNotFoundException(Guid userId)
        : base($"Profile not found for user ID: {userId}")
    {
    }
}

public class ProfileIncompleteException : DomainException
{
    public ProfileIncompleteException(string message)
        : base(message)
    {
    }
}

public class InvalidProfileDataException : DomainException
{
    public InvalidProfileDataException(string message)
        : base(message)
    {
    }
}

public class ProfileAlreadyExistsException : DomainException
{
    public ProfileAlreadyExistsException(Guid userId)
        : base($"Profile already exists for user ID: {userId}")
    {
    }
}