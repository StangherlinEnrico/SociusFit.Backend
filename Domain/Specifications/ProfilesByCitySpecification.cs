using Domain.Entities;

namespace Domain.Specifications;

public class ProfilesByCitySpecification : BaseSpecification<Profile>
{
    public ProfilesByCitySpecification(string city)
        : base(p => p.City.ToLower() == city.ToLower())
    {
    }
}