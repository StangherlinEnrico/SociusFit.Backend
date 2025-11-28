using Domain.Constants;
using Domain.Entities;

namespace Domain.Validators;

public class SportValidator
{
    public ValidationResult Validate(Sport sport)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(sport.Name))
            errors.Add("Sport name is required");
        else if (sport.Name.Length < SportConstants.MinSportNameLength)
            errors.Add($"Sport name must be at least {SportConstants.MinSportNameLength} characters long");
        else if (sport.Name.Length > SportConstants.MaxSportNameLength)
            errors.Add($"Sport name cannot exceed {SportConstants.MaxSportNameLength} characters");

        if (!Enum.IsDefined(typeof(Enums.SportLevel), sport.Level))
            errors.Add("Invalid sport level");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    public ValidationResult ValidateSportsList(IEnumerable<Sport> sports)
    {
        var errors = new List<string>();
        var sportsList = sports.ToList();

        if (sportsList.Count < ProfileConstants.MinSportsRequired)
            errors.Add($"At least {ProfileConstants.MinSportsRequired} sport is required");

        if (sportsList.Count > ProfileConstants.MaxSportsAllowed)
            errors.Add($"Cannot have more than {ProfileConstants.MaxSportsAllowed} sports");

        var duplicateNames = sportsList
            .GroupBy(s => s.Name.ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
            errors.Add($"Duplicate sports found: {string.Join(", ", duplicateNames)}");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}