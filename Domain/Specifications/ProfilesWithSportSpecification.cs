using Domain.Entities;

namespace Domain.Specifications;

public class ProfilesWithSportSpecification : BaseSpecification<Profile>
{
    public ProfilesWithSportSpecification(Guid sportId)
        : base(p => p.ProfileSports.Any(ps => ps.SportId == sportId))
    {
    }

    public ProfilesWithSportSpecification(string sportName)
        : base(p => p.ProfileSports.Any(ps => ps.Sport.Name.ToLower() == sportName.ToLower()))
    {
    }
}