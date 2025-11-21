using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;

/// <summary>
/// Validator for OAuthLoginDto
/// </summary>
public class OAuthLoginDtoValidator : AbstractValidator<OAuthLoginDto>
{
    public OAuthLoginDtoValidator()
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