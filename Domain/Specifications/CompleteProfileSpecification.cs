using Domain.Entities;

namespace Domain.Specifications;

public class CompleteProfileSpecification : BaseSpecification<Profile>
{
    public CompleteProfileSpecification()
        : base(p =>
            p.Age >= 18 &&
            !string.IsNullOrWhiteSpace(p.Gender) &&
            !string.IsNullOrWhiteSpace(p.City) &&
            !string.IsNullOrWhiteSpace(p.Bio) &&
            !string.IsNullOrWhiteSpace(p.PhotoUrl) &&
            p.Sports.Count > 0)
    {
    }
}