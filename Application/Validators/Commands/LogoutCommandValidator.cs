using Application.Features.Auth.Commands.Logout;
using FluentValidation;

namespace Application.Validators.Commands;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Invalid user");

        When(x => !x.RevokeAll, () =>
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required when not revoking all tokens");
        });
    }
}