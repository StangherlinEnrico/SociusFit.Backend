using Application.DTOs.Users;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Users;

/// <summary>
/// Validator for UpdateLocationDto
/// </summary>
public class UpdateLocationDtoValidator : AbstractValidator<UpdateLocationDto>
{
    public UpdateLocationDtoValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Location), () =>
        {
            RuleFor(x => x.Location)
                .MaximumLength(ValidationConstants.Location.MaxLength)
                .WithMessage($"Location cannot exceed {ValidationConstants.Location.MaxLength} characters");
        });

        When(x => x.MaxDistance.HasValue, () =>
        {
            RuleFor(x => x.MaxDistance)
                .GreaterThanOrEqualTo(ValidationConstants.Location.MinDistance)
                .WithMessage($"Max distance must be at least {ValidationConstants.Location.MinDistance} km")
                .LessThanOrEqualTo(ValidationConstants.Location.MaxDistanceLimit)
                .WithMessage($"Max distance cannot exceed {ValidationConstants.Location.MaxDistanceLimit} km");
        });
    }
}