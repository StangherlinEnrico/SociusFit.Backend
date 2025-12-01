namespace Application.Requests;

public class UpdateProfileRequest
{
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public int MaxDistance { get; set; }
    public List<AddSportRequest> Sports { get; set; } = new();
}