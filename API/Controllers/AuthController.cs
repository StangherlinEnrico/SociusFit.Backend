using Application.DTOs.Users;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.LoginOAuth;
using Application.Features.Users.Commands.Register;
using Application.Features.Users.Commands.ResendVerification;
using Application.Features.Users.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication endpoints for mobile app
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">User registration data</param>
    /// <returns>Authentication response with JWT tokens</returns>
    /// <response code="200">User registered successfully. Verification email sent.</response>
    /// <response code="400">Invalid input or email already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        var command = new RegisterUserCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = dto.Password,
            IpAddress = GetIpAddress()
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
    /// <returns>Authentication response with JWT tokens</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var command = new LoginCommand
        {
            Email = dto.Email,
            Password = dto.Password,
            IpAddress = GetIpAddress()
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
    /// Login with OAuth provider (Google, Apple)
    /// </summary>
    /// <param name="dto">OAuth login data</param>
    /// <returns>Authentication response with JWT tokens</returns>
    /// <response code="200">OAuth login successful</response>
    /// <response code="401">Invalid OAuth token</response>
    [HttpPost("login/oauth")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginOAuth([FromBody] OAuthLoginDto dto)
    {
        var command = new LoginOAuthCommand
        {
            Provider = dto.Provider,
            Token = dto.Token,
            IpAddress = GetIpAddress()
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
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="dto">Refresh token data</param>
    /// <returns>New JWT tokens</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = dto.RefreshToken,
            IpAddress = GetIpAddress()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Token refresh failed");
            return Unauthorized(result);
        }

        _logger.LogInformation("Token refreshed successfully");
        return Ok(result);
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    /// <param name="dto">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="400">Invalid refresh token</response>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(Application.Common.Models.Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto)
    {
        var command = new RevokeTokenCommand
        {
            RefreshToken = dto.RefreshToken,
            IpAddress = GetIpAddress()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Token revocation failed");
            return BadRequest(result);
        }

        _logger.LogInformation("Token revoked successfully");
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
    [AllowAnonymous]
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

        _logger.LogInformation("Email verified successfully");

        // Return HTML page for better user experience
        return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Email Verified - SociusFit</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            display: flex; 
            justify-content: center; 
            align-items: center; 
            min-height: 100vh; 
            margin: 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
        }}
        .container {{ 
            background: white; 
            padding: 40px; 
            border-radius: 20px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            text-align: center;
            max-width: 500px;
            width: 100%;
        }}
        h1 {{ 
            color: #667eea; 
            margin-bottom: 20px;
            font-size: 28px;
        }}
        p {{ 
            color: #555; 
            line-height: 1.6; 
            margin-bottom: 30px;
            font-size: 16px;
        }}
        .success-icon {{ 
            font-size: 60px; 
            margin-bottom: 20px; 
        }}
        .btn {{ 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px 30px;
            border: none;
            border-radius: 10px;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s;
        }}
        .btn:hover {{ 
            transform: translateY(-2px);
        }}
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
    [AllowAnonymous]
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

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}