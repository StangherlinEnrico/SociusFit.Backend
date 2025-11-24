namespace Application.DTOs.Users;

/// <summary>
/// DTO for updating user location settings
/// </summary>
public class UpdateLocationDto
{
    /// <summary>
    /// City or location (e.g., "Treviso")
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Maximum distance in kilometers the user is willing to travel (e.g., 15)
    /// </summary>
    public int? MaxDistance { get; set; }
}