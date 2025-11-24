using Application.Features.Users.Commands.LoginOAuth;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Commands;

public class LoginOAuthCommandValidator : AbstractValidator<LoginOAuthCommand>
{
    public LoginOAuthCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required")
            .Must(AuthConstants.OAuthProviders.IsSupported)
            .WithMessage($"Provider must be one of: {string.Join(", ", AuthConstants.OAuthProviders.Supported)}");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        // Apple-specific: names may be provided separately
        When(x => x.Provider.Equals(AuthConstants.OAuthProviders.Apple, StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(ValidationConstants.User.FirstNameMaxLength)
                .WithMessage($"First name cannot exceed {ValidationConstants.User.FirstNameMaxLength} characters");

            RuleFor(x => x.LastName)
                .MaximumLength(ValidationConstants.User.LastNameMaxLength)
                .WithMessage($"Last name cannot exceed {ValidationConstants.User.LastNameMaxLength} characters");
        });
    }
}