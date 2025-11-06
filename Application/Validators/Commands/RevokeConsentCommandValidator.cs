using Application.Features.Consents.Commands.Revoke;
using FluentValidation;

namespace Application.Validators.Commands;

public class RevokeConsentCommandValidator : AbstractValidator<RevokeConsentCommand>
{
    public RevokeConsentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.ConsentType)
            .NotEmpty().WithMessage("Consent type is required");
    }
}