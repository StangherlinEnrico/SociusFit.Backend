using Application.DTOs.Users;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Users;

public class OAuthLoginDtoValidator : AbstractValidator<OAuthLoginDto>
{
    public OAuthLoginDtoValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required")
            .Must(AuthConstants.OAuthProviders.IsSupported)
            .WithMessage($"Provider must be one of: {string.Join(", ", AuthConstants.OAuthProviders.Supported)}");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        When(x => !string.IsNullOrEmpty(x.FirstName), () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(ValidationConstants.User.FirstNameMaxLength)
                .WithMessage($"First name cannot exceed {ValidationConstants.User.FirstNameMaxLength} characters");
        });

        When(x => !string.IsNullOrEmpty(x.LastName), () =>
        {
            RuleFor(x => x.LastName)
                .MaximumLength(ValidationConstants.User.LastNameMaxLength)
                .WithMessage($"Last name cannot exceed {ValidationConstants.User.LastNameMaxLength} characters");
        });
    }
}