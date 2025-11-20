using Application.Features.Users.Commands.LoginOAuth;
using FluentValidation;

namespace Application.Validators.Commands;

public class LoginOAuthCommandValidator : AbstractValidator<LoginOAuthCommand>
{
    public LoginOAuthCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required")
            .Must(p => new[] { "google", "facebook", "apple", "microsoft" }
                .Contains(p.ToLower()))
            .WithMessage("Provider must be one of: google, facebook, apple, microsoft");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}
