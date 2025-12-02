using Domain.Enums;

namespace Application.Requests;

public class CreateProfileRequest
{
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty; // Formato: "Comune (Regione)" es: "Milano (Lombardia)"
    public string Bio { get; set; } = string.Empty;
    public int MaxDistance { get; set; } = Domain.Constants.ProfileConstants.DefaultMaxDistance;
    public List<AddSportRequest> Sports { get; set; } = new();
}