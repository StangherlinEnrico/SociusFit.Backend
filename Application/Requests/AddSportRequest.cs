using Domain.Enums;

namespace Application.Requests;

public class AddSportRequest
{
    public Guid SportId { get; set; }
    public SportLevel Level { get; set; }
}