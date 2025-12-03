using Application.UseCases.Matches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly GetMatchesUseCase _getMatchesUseCase;

    public MatchesController(GetMatchesUseCase getMatchesUseCase)
    {
        _getMatchesUseCase = getMatchesUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetMatches(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _getMatchesUseCase.ExecuteAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }
}