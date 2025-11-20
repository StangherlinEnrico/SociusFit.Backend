using System.Security.Claims;
using Application.DTOs.Users;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.UpdateLocation;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Users management endpoints (protected)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize] // All endpoints require authentication
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current authenticated user profile
    /// </summary>
    /// <returns>User profile</returns>
    /// <response code="200">User found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var query = new GetUserByIdQuery { UserId = userId.Value };
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    /// <response code="200">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(int id)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="dto">Profile update data</param>
    /// <returns>Updated user</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpPut("me/profile")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var command = new UpdateUserProfileCommand
        {
            UserId = userId.Value,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Location = dto.Location
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Profile updated for user: {UserId}", userId.Value);
        return Ok(result);
    }

    /// <summary>
    /// Update current user location
    /// </summary>
    /// <param name="dto">Location update data</param>
    /// <returns>Updated user</returns>
    /// <response code="200">Location updated successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpPut("me/location")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateUserLocationDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var command = new UpdateUserLocationCommand
        {
            UserId = userId.Value,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            MaxDistanceKm = dto.MaxDistanceKm
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Location updated for user: {UserId}", userId.Value);
        return Ok(result);
    }

    /// <summary>
    /// Delete current user account (soft delete)
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">User deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpDelete("me")]
    [ProducesResponseType(typeof(Application.Common.Models.Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var command = new DeleteUserCommand { UserId = userId.Value };
        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("User deleted: {UserId}", userId.Value);
        return Ok(result);
    }

    private int? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}