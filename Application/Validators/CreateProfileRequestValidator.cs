using Application.Requests;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileRequestValidator()
    {
        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(ProfileConstants.MinAge).WithMessage($"Age must be at least {ProfileConstants.MinAge} years old")
            .LessThanOrEqualTo(ProfileConstants.MaxAge).WithMessage($"Age cannot exceed {ProfileConstants.MaxAge} years");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MinimumLength(ProfileConstants.MinCityLength).WithMessage($"City must be at least {ProfileConstants.MinCityLength} characters")
            .MaximumLength(ProfileConstants.MaxCityLength).WithMessage($"City cannot exceed {ProfileConstants.MaxCityLength} characters");

        RuleFor(x => x.Bio)
            .NotEmpty().WithMessage("Bio is required")
            .MinimumLength(ProfileConstants.MinBioLength).WithMessage($"Bio must be at least {ProfileConstants.MinBioLength} characters")
            .MaximumLength(ProfileConstants.MaxBioLength).WithMessage($"Bio cannot exceed {ProfileConstants.MaxBioLength} characters");

        RuleFor(x => x.Sports)
            .NotEmpty().WithMessage("At least one sport is required")
            .Must(sports => sports.Count >= ProfileConstants.MinSportsRequired).WithMessage($"At least {ProfileConstants.MinSportsRequired} sport is required")
            .Must(sports => sports.Count <= ProfileConstants.MaxSportsAllowed).WithMessage($"Cannot have more than {ProfileConstants.MaxSportsAllowed} sports");

        RuleForEach(x => x.Sports).SetValidator(new CreateSportRequestValidator());
    }
}

public class CreateSportRequestValidator : AbstractValidator<CreateSportRequest>
{
    public CreateSportRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sport name is required")
            .MinimumLength(SportConstants.MinSportNameLength).WithMessage($"Sport name must be at least {SportConstants.MinSportNameLength} characters")
            .MaximumLength(SportConstants.MaxSportNameLength).WithMessage($"Sport name cannot exceed {SportConstants.MaxSportNameLength} characters");

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Invalid sport level");
    }
}