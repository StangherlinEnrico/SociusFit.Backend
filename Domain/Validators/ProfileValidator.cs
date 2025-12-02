using Domain.Constants;
using Domain.Entities;

namespace Domain.Validators;

public class ProfileValidator
{
    public ValidationResult Validate(Profile profile)
    {
        var errors = new List<string>();

        if (profile.Age < ProfileConstants.MinAge)
            errors.Add($"Age must be at least {ProfileConstants.MinAge} years old");

        if (profile.Age > ProfileConstants.MaxAge)
            errors.Add($"Age cannot exceed {ProfileConstants.MaxAge} years");

        if (string.IsNullOrWhiteSpace(profile.Gender))
            errors.Add("Gender is required");

        if (string.IsNullOrWhiteSpace(profile.City))
            errors.Add("City is required");
        else if (profile.City.Length < ProfileConstants.MinCityLength)
            errors.Add($"City must be at least {ProfileConstants.MinCityLength} characters long");
        else if (profile.City.Length > ProfileConstants.MaxCityLength)
            errors.Add($"City cannot exceed {ProfileConstants.MaxCityLength} characters");

        // Validazione coordinate (calcolate internamente dal geocoding service)
        if (profile.Latitude < ProfileConstants.MinLatitude || profile.Latitude > ProfileConstants.MaxLatitude)
            errors.Add($"Latitude must be between {ProfileConstants.MinLatitude} and {ProfileConstants.MaxLatitude}");

        if (profile.Longitude < ProfileConstants.MinLongitude || profile.Longitude > ProfileConstants.MaxLongitude)
            errors.Add($"Longitude must be between {ProfileConstants.MinLongitude} and {ProfileConstants.MaxLongitude}");

        if (string.IsNullOrWhiteSpace(profile.Bio))
            errors.Add("Bio is required");
        else if (profile.Bio.Length < ProfileConstants.MinBioLength)
            errors.Add($"Bio must be at least {ProfileConstants.MinBioLength} characters long");
        else if (profile.Bio.Length > ProfileConstants.MaxBioLength)
            errors.Add($"Bio cannot exceed {ProfileConstants.MaxBioLength} characters");

        if (profile.MaxDistance < ProfileConstants.MinMaxDistance)
            errors.Add($"Max distance must be at least {ProfileConstants.MinMaxDistance} km");

        if (profile.MaxDistance > ProfileConstants.MaxMaxDistance)
            errors.Add($"Max distance cannot exceed {ProfileConstants.MaxMaxDistance} km");

        if (profile.ProfileSports == null || profile.ProfileSports.Count < ProfileConstants.MinSportsRequired)
            errors.Add($"At least {ProfileConstants.MinSportsRequired} sport is required");

        if (profile.ProfileSports != null && profile.ProfileSports.Count > ProfileConstants.MaxSportsAllowed)
            errors.Add($"Cannot have more than {ProfileConstants.MaxSportsAllowed} sports");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    public ValidationResult ValidateForCompletion(Profile profile)
    {
        var basicValidation = Validate(profile);
        if (!basicValidation.IsSuccess)
            return basicValidation;

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.PhotoUrl))
            errors.Add("Profile photo is required for completion");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}