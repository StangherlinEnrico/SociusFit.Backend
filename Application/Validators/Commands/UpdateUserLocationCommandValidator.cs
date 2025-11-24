using Application.Features.Users.Commands.UpdateLocation;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Commands;

/// <summary>
/// Validator for UpdateUserLocationCommand
/// </summary>
public class UpdateUserLocationCommandValidator : AbstractValidator<UpdateUserLocationCommand>
{
    public UpdateUserLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

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