using System.Security.Claims;
using Application.DTOs.Users;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Users.Commands.ChangePassword;
using Application.Features.Users.Commands.ForgotPassword;
using Application.Features.Users.Commands.Login;
using Application.Features.Users.Commands.LoginOAuth;
using Application.Features.Users.Commands.Register;
using Application.Features.Users.Commands.ResendVerification;
using Application.Features.Users.Commands.ResetPassword;
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

        _logger.LogInformation("User logged in: {Email}, UserId: {UserId}", dto.Email, result.Data?.User.Id);
        return Ok(result);
    }

    /// <summary>
    /// Login with OAuth provider (Google, Apple)
    /// </summary>
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
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IpAddress = GetIpAddress()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("OAuth login failed for provider: {Provider}", dto.Provider);
            return Unauthorized(result);
        }

        _logger.LogInformation("User logged in via OAuth: {Provider}, UserId: {UserId}",
            dto.Provider, result.Data?.User.Id);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
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

        return Ok(result);
    }

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
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
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout user (revoke all refresh tokens)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(Application.Common.Models.Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var command = new LogoutCommand
        {
            UserId = userId.Value,
            IpAddress = GetIpAddress(),
            RevokeAll = true
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        _logger.LogInformation("User logged out: {UserId}", userId.Value);
        return Ok(result);
    }

    /// <summary>
    /// Verify user email with token
    /// </summary>
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
            _logger.LogWarning("Email verification failed");
            return BadRequest(result);
        }

        _logger.LogInformation("Email verified successfully");

        // Return HTML page for better user experience when opened in browser
        return Content(GetVerificationSuccessHtml(result.Message ?? "Email verified successfully"), "text/html");
    }

    /// <summary>
    /// Resend verification email
    /// </summary>
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
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var command = new ForgotPasswordCommand { Email = dto.Email };
        var result = await _mediator.Send(command);

        // Always return success to prevent email enumeration
        return Ok(result);
    }

    /// <summary>
    /// Reset password using token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Common.Models.Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var command = new ResetPasswordCommand
        {
            Token = dto.Token,
            NewPassword = dto.NewPassword,
            ConfirmPassword = dto.ConfirmPassword
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        _logger.LogInformation("Password reset successful");
        return Ok(result);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(Application.Common.Models.Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return Unauthorized();

        var command = new ChangePasswordCommand
        {
            UserId = userId.Value,
            CurrentPassword = dto.CurrentPassword,
            NewPassword = dto.NewPassword,
            ConfirmPassword = dto.ConfirmPassword,
            IpAddress = GetIpAddress()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        _logger.LogInformation("Password changed for user: {UserId}", userId.Value);
        return Ok(result);
    }

    private int? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private string GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string GetVerificationSuccessHtml(string message)
    {
        return $@"
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
            box-sizing: border-box;
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
        h1 {{ color: #667eea; margin-bottom: 20px; font-size: 28px; }}
        p {{ color: #555; line-height: 1.6; margin-bottom: 30px; font-size: 16px; }}
        .success-icon {{ font-size: 60px; margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='success-icon'>✅</div>
        <h1>Email Verified!</h1>
        <p>{message}</p>
        <p>You can now close this page and return to the app.</p>
    </div>
</body>
</html>";
    }
}