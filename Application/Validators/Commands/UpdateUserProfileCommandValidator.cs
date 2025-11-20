using Application.Features.Users.Commands.UpdateProfile;
using FluentValidation;

namespace Application.Validators.Commands;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Location)
            .MaximumLength(255).WithMessage("Location cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));
    }
}
