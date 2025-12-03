namespace Application.Responses;

public class SwipeLikeResponse
{
    public bool IsMatch { get; set; }
    public Guid? MatchId { get; set; }
    public string? MatchedUserName { get; set; }
}