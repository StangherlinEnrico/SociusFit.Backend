using Application.DTOs.Users;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Users;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(ValidationConstants.User.EmailMaxLength)
            .WithMessage($"Email cannot exceed {ValidationConstants.User.EmailMaxLength} characters");
    }
}