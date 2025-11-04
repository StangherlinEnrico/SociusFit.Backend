using Application.DTOs.Consents;
using Application.DTOs.Sports;
using FluentValidation;

namespace Application.Validators.Sports;

/// <summary>
/// Validator for CreateSportDto
/// </summary>
public class CreateSportDtoValidator : AbstractValidator<CreateSportDto>
{
    public CreateSportDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sport name is required")
            .MaximumLength(100).WithMessage("Sport name cannot exceed 100 characters");
    }
}

/// <summary>
/// Validator for CreateLevelDto
/// </summary>
public class CreateLevelDtoValidator : AbstractValidator<CreateLevelDto>
{
    public CreateLevelDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Level name is required")
            .MaximumLength(100).WithMessage("Level name cannot exceed 100 characters");
    }
}

/// <summary>
/// Validator for AddUserSportDto
/// </summary>
public class AddUserSportDtoValidator : AbstractValidator<AddUserSportDto>
{
    public AddUserSportDtoValidator()
    {
        RuleFor(x => x.SportId)
            .GreaterThan(0).WithMessage("Sport ID must be greater than 0");

        RuleFor(x => x.LevelId)
            .GreaterThan(0).WithMessage("Level ID must be greater than 0");
    }
}

/// <summary>
/// Validator for UpdateUserSportDto
/// </summary>
public class UpdateUserSportDtoValidator : AbstractValidator<UpdateUserSportDto>
{
    public UpdateUserSportDtoValidator()
    {
        RuleFor(x => x.LevelId)
            .GreaterThan(0).WithMessage("Level ID must be greater than 0");
    }
}
