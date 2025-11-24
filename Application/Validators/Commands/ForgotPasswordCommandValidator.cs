using Application.Features.Users.Commands.ForgotPassword;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Commands;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(ValidationConstants.User.EmailMaxLength)
            .WithMessage($"Email cannot exceed {ValidationConstants.User.EmailMaxLength} characters");
    }
}