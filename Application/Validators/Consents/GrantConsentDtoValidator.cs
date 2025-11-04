using Application.DTOs.Consents;
using FluentValidation;

namespace Application.Validators.Consents;

/// <summary>
/// Validator for GrantConsentDto
/// </summary>
public class GrantConsentDtoValidator : AbstractValidator<GrantConsentDto>
{
    public GrantConsentDtoValidator()
    {
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

/// <summary>
/// Validator for RevokeConsentDto
/// </summary>
public class RevokeConsentDtoValidator : AbstractValidator<RevokeConsentDto>
{
    public RevokeConsentDtoValidator()
    {
        RuleFor(x => x.ConsentType)
            .NotEmpty().WithMessage("Consent type is required");
    }
}