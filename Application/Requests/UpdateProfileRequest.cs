using Domain.Enums;

namespace Application.Requests;

public class UpdateProfileRequest
{
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<UpdateSportRequest> Sports { get; set; } = new();
}

public class UpdateSportRequest
{
    public string Name { get; set; } = string.Empty;
    public SportLevel Level { get; set; }
}