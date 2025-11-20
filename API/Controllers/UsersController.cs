using Application.DTOs.Users;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.LoginOAuth;
using Application.Features.Users.Commands.Register;
using Application.Features.Users.Commands.ResendVerification;
using Application.Features.Users.Commands.UpdateLocation;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.Commands.VerifyEmail;
using Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    /// <returns>Authentication response with token and verification message</returns>
    /// <response code="200">User registered successfully. Verification email sent.</response>
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

        _logger.LogInformation("User registered successfully: {Email}. Verification email sent.", dto.Email);
        return Ok(result);
    }

    /// <summary>
    /// Verify user email with token
    /// </summary>
    /// <param name="token">Email verification token</param>
    /// <returns>Verification confirmation</returns>
    /// <response code="200">Email verified successfully</response>
    /// <response code="400">Invalid or expired token</response>
    [HttpGet("verify-email")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var command = new VerifyEmailCommand { Token = token };
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Email verification failed for token: {Token}", token);
            return BadRequest(result);
        }

        _logger.LogInformation("Email verified successfully for token: {Token}", token);

        // Return HTML page for better user experience
        return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Email Verified - SociusFit</title>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            display: flex; 
            justify-content: center; 
            align-items: center; 
            height: 100vh; 
            margin: 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }}
        .container {{ 
            background: white; 
            padding: 40px; 
            border-radius: 10px; 
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            text-align: center;
            max-width: 500px;
        }}
        h1 {{ color: #667eea; margin-bottom: 20px; }}
        p {{ color: #555; line-height: 1.6; margin-bottom: 30px; }}
        .success-icon {{ font-size: 60px; margin-bottom: 20px; }}
        .btn {{ 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px 30px;
            border: none;
            border-radius: 5px;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
        }}
        .btn:hover {{ opacity: 0.9; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='success-icon'>✅</div>
        <h1>Email Verified Successfully!</h1>
        <p>{result.Message}</p>
        <p>You can now close this window and return to the app to log in.</p>
        <a href='#' class='btn' onclick='window.close(); return false;'>Close Window</a>
    </div>
</body>
</html>", "text/html");
    }

    /// <summary>
    /// Resend verification email
    /// </summary>
    /// <param name="dto">Email to resend verification to</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Verification email sent</response>
    /// <response code="400">Email already verified or other error</response>
    [HttpPost("resend-verification")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var command = new ResendVerificationEmailCommand { Email = dto.Email };
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Resend verification failed for email: {Email}", dto.Email);
            return BadRequest(result);
        }

        _logger.LogInformation("Verification email resent to: {Email}", dto.Email);
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