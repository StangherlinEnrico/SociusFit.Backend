using Application.Features.Users.Commands.Register;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(ValidationConstants.User.FirstNameMaxLength)
            .WithMessage($"First name cannot exceed {ValidationConstants.User.FirstNameMaxLength} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(ValidationConstants.User.LastNameMaxLength)
            .WithMessage($"Last name cannot exceed {ValidationConstants.User.LastNameMaxLength} characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(ValidationConstants.User.EmailMaxLength)
            .WithMessage($"Email cannot exceed {ValidationConstants.User.EmailMaxLength} characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(ValidationConstants.User.PasswordMinLength)
            .WithMessage($"Password must be at least {ValidationConstants.User.PasswordMinLength} characters")
            .MaximumLength(ValidationConstants.User.PasswordMaxLength)
            .WithMessage($"Password cannot exceed {ValidationConstants.User.PasswordMaxLength} characters");
    }
}