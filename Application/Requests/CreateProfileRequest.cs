using Domain.Enums;

namespace Application.Requests;

public class CreateProfileRequest
{
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<CreateSportRequest> Sports { get; set; } = new();
}

public class CreateSportRequest
{
    public string Name { get; set; } = string.Empty;
    public SportLevel Level { get; set; }
}