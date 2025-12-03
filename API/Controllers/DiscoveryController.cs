using Application.Requests;
using Application.UseCases.Discovery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/discovery")]
[Authorize]
public class DiscoveryController : ControllerBase
{
    private readonly GetDiscoveryCardsUseCase _getDiscoveryCardsUseCase;
    private readonly SwipeLikeUseCase _swipeLikeUseCase;

    public DiscoveryController(
        GetDiscoveryCardsUseCase getDiscoveryCardsUseCase,
        SwipeLikeUseCase swipeLikeUseCase)
    {
        _getDiscoveryCardsUseCase = getDiscoveryCardsUseCase;
        _swipeLikeUseCase = swipeLikeUseCase;
    }

    [HttpGet("cards")]
    public async Task<IActionResult> GetCards(
        [FromQuery] Guid? sportId,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var request = new GetDiscoveryCardsRequest
        {
            SportId = sportId,
            PageSize = Math.Min(pageSize, 50),
            PageNumber = Math.Max(pageNumber, 1)
        };

        var result = await _getDiscoveryCardsUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpPost("like")]
    public async Task<IActionResult> Like(
        [FromBody] SwipeLikeRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _swipeLikeUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }
}