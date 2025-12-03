using Application.Requests;
using Application.UseCases.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly RegisterDeviceTokenUseCase _registerDeviceTokenUseCase;

    public NotificationsController(RegisterDeviceTokenUseCase registerDeviceTokenUseCase)
    {
        _registerDeviceTokenUseCase = registerDeviceTokenUseCase;
    }

    [HttpPost("register-token")]
    public async Task<IActionResult> RegisterDeviceToken(
        [FromBody] RegisterDeviceTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _registerDeviceTokenUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(new { success = true });
    }
}