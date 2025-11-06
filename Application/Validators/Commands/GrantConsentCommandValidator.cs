using Application.Features.Consents.Commands.Grant;
using FluentValidation;

namespace Application.Validators.Commands;

public class GrantConsentCommandValidator : AbstractValidator<GrantConsentCommand>
{
    public GrantConsentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.ConsentType)
            .NotEmpty().WithMessage("Consent type is required")
            .Must(ct => new[]
            {
                "terms_of_service",
                "privacy_policy",
                "marketing_emails",
                "data_processing",
                "location_sharing"
            }.Contains(ct))
            .WithMessage("Invalid consent type");
    }
}