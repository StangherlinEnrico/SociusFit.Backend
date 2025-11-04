using Application.Features.Sports.Commands.Create;
using Application.Features.UserSports.Commands.Add;
using Application.Features.UserSports.Commands.Remove;
using Application.Features.UserSports.Commands.UpdateLevel;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.LoginOAuth;
using Application.Features.Users.Commands.Register;
using Application.Features.Users.Commands.UpdateLocation;
using Application.Features.Users.Commands.UpdateProfile;
using FluentValidation;

namespace Application.Validators.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters");
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

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

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Location)
            .MaximumLength(255).WithMessage("Location cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));
    }
}

public class UpdateUserLocationCommandValidator : AbstractValidator<UpdateUserLocationCommand>
{
    public UpdateUserLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.MaxDistanceKm)
            .GreaterThan(0).WithMessage("Max distance must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max distance cannot exceed 500 km");
    }
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");
    }
}

public class CreateSportCommandValidator : AbstractValidator<CreateSportCommand>
{
    public CreateSportCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sport name is required")
            .MaximumLength(100).WithMessage("Sport name cannot exceed 100 characters");
    }
}

public class AddUserSportCommandValidator : AbstractValidator<AddUserSportCommand>
{
    public AddUserSportCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.SportId)
            .GreaterThan(0).WithMessage("Sport ID must be greater than 0");

        RuleFor(x => x.LevelId)
            .GreaterThan(0).WithMessage("Level ID must be greater than 0");
    }
}

public class UpdateUserSportLevelCommandValidator : AbstractValidator<UpdateUserSportLevelCommand>
{
    public UpdateUserSportLevelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.SportId)
            .GreaterThan(0).WithMessage("Sport ID must be greater than 0");

        RuleFor(x => x.NewLevelId)
            .GreaterThan(0).WithMessage("Level ID must be greater than 0");
    }
}

public class RemoveUserSportCommandValidator : AbstractValidator<RemoveUserSportCommand>
{
    public RemoveUserSportCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.SportId)
            .GreaterThan(0).WithMessage("Sport ID must be greater than 0");
    }
}