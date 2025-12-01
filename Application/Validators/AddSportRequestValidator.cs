using Application.Requests;
using FluentValidation;

namespace Application.Validators;

public class AddSportRequestValidator : AbstractValidator<AddSportRequest>
{
    public AddSportRequestValidator()
    {
        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required");

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Invalid sport level");
    }
}