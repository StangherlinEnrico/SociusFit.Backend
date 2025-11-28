namespace Domain.Exceptions;

public class InvalidAgeException : DomainException
{
    public InvalidAgeException(string message)
        : base(message)
    {
    }
}

public class InvalidBioException : DomainException
{
    public InvalidBioException(string message)
        : base(message)
    {
    }
}

public class InvalidCityException : DomainException
{
    public InvalidCityException(string message)
        : base(message)
    {
    }
}

public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message)
        : base(message)
    {
    }
}