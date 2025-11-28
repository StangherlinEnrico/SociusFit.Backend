using Domain.Entities;

namespace Domain.Specifications;

public class ProfilesWithSportSpecification : BaseSpecification<Profile>
{
    public ProfilesWithSportSpecification(Guid sportId)
        : base(p => p.Sports.Any(s => s.Id == sportId))
    {
    }

    public ProfilesWithSportSpecification(string sportName)
        : base(p => p.Sports.Any(s => s.Name.ToLower() == sportName.ToLower()))
    {
    }
}