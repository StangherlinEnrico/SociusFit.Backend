using Application.Requests;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(UserConstants.MinNameLength).WithMessage($"First name must be at least {UserConstants.MinNameLength} characters")
            .MaximumLength(UserConstants.MaxNameLength).WithMessage($"First name cannot exceed {UserConstants.MaxNameLength} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(UserConstants.MinNameLength).WithMessage($"Last name must be at least {UserConstants.MinNameLength} characters")
            .MaximumLength(UserConstants.MaxNameLength).WithMessage($"Last name cannot exceed {UserConstants.MaxNameLength} characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(UserConstants.MinPasswordLength).WithMessage($"Password must be at least {UserConstants.MinPasswordLength} characters")
            .MaximumLength(UserConstants.MaxPasswordLength).WithMessage($"Password cannot exceed {UserConstants.MaxPasswordLength} characters");
    }
}