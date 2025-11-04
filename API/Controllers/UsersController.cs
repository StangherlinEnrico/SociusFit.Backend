using Application.DTOs.Users;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.LoginOAuth;
using Application.Features.Users.Commands.Register;
using Application.Features.Users.Commands.UpdateLocation;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.Queries.GetUser;
using Application.Features.Users.Queries.GetUserWithSports;
using Application.Features.Users.Queries.SearchNearby;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers;

/// <summary>
/// Users management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
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
    /// Register a new user
    /// </summary>
    /// <param name="dto">User registration data</param>
    /// <returns>Authentication response with token</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid input or email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        var command = new RegisterUserCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = dto.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Registration failed for email: {Email}", dto.Email);
            return BadRequest(result);
        }

        _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication response with token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var command = new LoginCommand
        {
            Email = dto.Email,
            Password = dto.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Login failed for email: {Email}", dto.Email);
            return Unauthorized(result);
        }

        _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
        return Ok(result);
    }

    /// <summary>
    /// Login with OAuth provider (Google, Facebook, Microsoft, Apple)
    /// </summary>
    /// <param name="dto">OAuth login data</param>
    /// <returns>Authentication response with token</returns>
    /// <response code="200">OAuth login successful</response>
    /// <response code="401">Invalid OAuth token</response>
    [HttpPost("login/oauth")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginOAuth([FromBody] OAuthLoginDto dto)
    {
        var command = new LoginOAuthCommand
        {
            Provider = dto.Provider,
            Token = dto.Token
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("OAuth login failed for provider: {Provider}", dto.Provider);
            return Unauthorized(result);
        }

        _logger.LogInformation("User logged in via OAuth: {Provider}", dto.Provider);
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
    /// Get user with their sports
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User with sports details</returns>
    /// <response code="200">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}/sports")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserWithSportsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserWithSports(int id)
    {
        var query = new GetUserWithSportsQuery { UserId = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Search for users nearby based on location
    /// </summary>
    /// <param name="latitude">User's latitude</param>
    /// <param name="longitude">User's longitude</param>
    /// <param name="maxDistanceKm">Maximum distance in kilometers (default: 50)</param>
    /// <param name="sportId">Filter by sport ID (optional)</param>
    /// <param name="levelId">Filter by level ID (optional)</param>
    /// <returns>List of nearby users</returns>
    /// <response code="200">Search completed successfully</response>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<List<UserSearchDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchNearby(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        [FromQuery] int maxDistanceKm = 50,
        [FromQuery] int? sportId = null,
        [FromQuery] int? levelId = null)
    {
        var query = new SearchNearbyUsersQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            MaxDistanceKm = maxDistanceKm,
            SportId = sportId,
            LevelId = levelId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="dto">Profile update data</param>
    /// <returns>Updated user</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="404">User not found</response>
    [HttpPut("{id}/profile")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserProfileDto dto)
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Location = dto.Location
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Profile updated for user: {UserId}", id);
        return Ok(result);
    }

    /// <summary>
    /// Update user location
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="dto">Location update data</param>
    /// <returns>Updated user</returns>
    /// <response code="200">Location updated successfully</response>
    /// <response code="404">User not found</response>
    [HttpPut("{id}/location")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateUserLocationDto dto)
    {
        var command = new UpdateUserLocationCommand
        {
            UserId = id,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            MaxDistanceKm = dto.MaxDistanceKm
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Location updated for user: {UserId}", id);
        return Ok(result);
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">User deleted successfully</response>
    /// <response code="404">User not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Application.Common.Models.Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("User deleted: {UserId}", id);
        return Ok(result);
    }
}