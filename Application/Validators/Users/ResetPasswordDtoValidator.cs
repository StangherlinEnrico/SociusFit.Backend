using Application.DTOs.Users;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Users;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(ValidationConstants.User.PasswordMinLength)
            .WithMessage($"Password must be at least {ValidationConstants.User.PasswordMinLength} characters")
            .MaximumLength(ValidationConstants.User.PasswordMaxLength)
            .WithMessage($"Password cannot exceed {ValidationConstants.User.PasswordMaxLength} characters");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}