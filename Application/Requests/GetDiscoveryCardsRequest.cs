namespace Application.Requests;

public class GetDiscoveryCardsRequest
{
    public Guid? SportId { get; set; }
    public int PageSize { get; set; } = 20;
    public int PageNumber { get; set; } = 1;
}