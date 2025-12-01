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

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}