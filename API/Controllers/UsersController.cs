using Application.Requests;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly LoginUserUseCase _loginUseCase;
    private readonly LogoutUserUseCase _logoutUseCase;
    private readonly DeleteAccountUseCase _deleteAccountUseCase;
    private readonly GetUserByIdUseCase _getUserByIdUseCase;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        RegisterUserUseCase registerUseCase,
        LoginUserUseCase loginUseCase,
        LogoutUserUseCase logoutUseCase,
        DeleteAccountUseCase deleteAccountUseCase,
        GetUserByIdUseCase getUserByIdUseCase,
        ILogger<UsersController> logger)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _logoutUseCase = logoutUseCase;
        _deleteAccountUseCase = deleteAccountUseCase;
        _getUserByIdUseCase = getUserByIdUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Application.DTOs.AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _registerUseCase.ExecuteAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.User.Id }, result.Value);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Application.DTOs.AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginUseCase.ExecuteAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    /// <remarks>
    /// Logs out the authenticated user by revoking the current JWT token.
    /// The token is added to a server-side blacklist, preventing its further use.
    /// 
    /// After calling this endpoint, the mobile client should:
    /// 1. Delete the stored JWT token from secure storage
    /// 2. Clear any cached user data
    /// 3. Redirect to the login screen
    /// 
    /// The revoked token will remain in the blacklist until it expires naturally.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        // Extract token from Authorization header
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new ErrorResponse { Errors = new List<string> { "Token not provided" } });
        }

        var result = await _logoutUseCase.ExecuteAsync(token, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Delete current user account (permanent)
    /// </summary>
    /// <remarks>
    /// PERMANENTLY deletes the authenticated user's account and ALL associated data:
    /// - User profile
    /// - Profile photo (from cloud storage)
    /// - Sports preferences
    /// - Credentials (password)
    /// - All tokens
    /// 
    /// This action is IRREVERSIBLE and requires password confirmation.
    /// 
    /// After successful deletion, the mobile client should:
    /// 1. Delete all stored user data
    /// 2. Clear cache
    /// 3. Redirect to welcome/registration screen
    /// 
    /// GDPR Compliance: User data is permanently removed from all systems.
    /// </remarks>
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAccount(
        [FromBody] DeleteAccountRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        // Extract current token for revocation
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new ErrorResponse { Errors = new List<string> { "Token not provided" } });
        }

        _logger.LogWarning("Account deletion requested for user {UserId}", userId);

        var result = await _deleteAccountUseCase.ExecuteAsync(userId, request, token, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        _logger.LogWarning("Account successfully deleted for user {UserId}", userId);

        return Ok(new
        {
            message = "Account successfully deleted. All your data has been permanently removed.",
            deletedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(Application.DTOs.UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _getUserByIdUseCase.ExecuteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(Application.DTOs.UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        var result = await _getUserByIdUseCase.ExecuteAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }
}