using Application.Features.Users.Commands.DeleteUser;
using FluentValidation;

namespace Application.Validators.Commands;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");
    }
}
