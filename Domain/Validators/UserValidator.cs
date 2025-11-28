using Domain.Constants;
using Domain.Entities;
using System.Text.RegularExpressions;

namespace Domain.Validators;

public class UserValidator
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public ValidationResult Validate(User user)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(user.FirstName))
            errors.Add("First name is required");
        else if (user.FirstName.Length < UserConstants.MinNameLength)
            errors.Add($"First name must be at least {UserConstants.MinNameLength} characters long");
        else if (user.FirstName.Length > UserConstants.MaxNameLength)
            errors.Add($"First name cannot exceed {UserConstants.MaxNameLength} characters");

        if (string.IsNullOrWhiteSpace(user.LastName))
            errors.Add("Last name is required");
        else if (user.LastName.Length < UserConstants.MinNameLength)
            errors.Add($"Last name must be at least {UserConstants.MinNameLength} characters long");
        else if (user.LastName.Length > UserConstants.MaxNameLength)
            errors.Add($"Last name cannot exceed {UserConstants.MaxNameLength} characters");

        if (string.IsNullOrWhiteSpace(user.Email))
            errors.Add("Email is required");
        else if (!EmailRegex.IsMatch(user.Email))
            errors.Add("Invalid email format");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    public ValidationResult ValidateEmail(string email)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required");
        else if (!EmailRegex.IsMatch(email))
            errors.Add("Invalid email format");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}