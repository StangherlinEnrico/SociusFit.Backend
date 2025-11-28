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
    private readonly GetUserByIdUseCase _getUserByIdUseCase;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        RegisterUserUseCase registerUseCase,
        LoginUserUseCase loginUseCase,
        GetUserByIdUseCase getUserByIdUseCase,
        ILogger<UsersController> logger)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
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