namespace Application.DTOs.Users;

/// <summary>
/// DTO for updating user profile
/// </summary>
public class UpdateUserProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Location { get; set; }
}
