namespace Application.DTOs.Users;

/// <summary>
/// DTO for updating user location
/// </summary>
public class UpdateUserLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int MaxDistanceKm { get; set; }
}
