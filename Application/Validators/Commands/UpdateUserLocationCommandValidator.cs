using Application.Features.Users.Commands.UpdateLocation;
using FluentValidation;

namespace Application.Validators.Commands;

public class UpdateUserLocationCommandValidator : AbstractValidator<UpdateUserLocationCommand>
{
    public UpdateUserLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.MaxDistanceKm)
            .GreaterThan(0).WithMessage("Max distance must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max distance cannot exceed 500 km");
    }
}
