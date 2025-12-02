using Application.Requests;
using FluentValidation;

namespace Application.Validators;

public class DeleteAccountRequestValidator : AbstractValidator<DeleteAccountRequest>
{
    public DeleteAccountRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required to confirm account deletion");

        RuleFor(x => x.ConfirmDeletion)
            .Equal(true)
            .WithMessage("You must explicitly confirm account deletion");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Deletion reason must not exceed 500 characters");
    }
}