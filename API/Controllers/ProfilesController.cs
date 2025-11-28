using Application.Requests;
using Application.UseCases.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly CreateProfileUseCase _createProfileUseCase;
    private readonly GetProfileByUserIdUseCase _getProfileUseCase;
    private readonly UpdateProfileUseCase _updateProfileUseCase;
    private readonly UploadProfilePhotoUseCase _uploadPhotoUseCase;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(
        CreateProfileUseCase createProfileUseCase,
        GetProfileByUserIdUseCase getProfileUseCase,
        UpdateProfileUseCase updateProfileUseCase,
        UploadProfilePhotoUseCase uploadPhotoUseCase,
        ILogger<ProfilesController> logger)
    {
        _createProfileUseCase = createProfileUseCase;
        _getProfileUseCase = getProfileUseCase;
        _updateProfileUseCase = updateProfileUseCase;
        _uploadPhotoUseCase = uploadPhotoUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Create profile for current user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Application.DTOs.ProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        var result = await _createProfileUseCase.ExecuteAsync(userId.Value, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return CreatedAtAction(nameof(GetMyProfile), null, result.Value);
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(Application.DTOs.ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        var result = await _getProfileUseCase.ExecuteAsync(userId.Value, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get profile by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(Application.DTOs.ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileByUserId([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _getProfileUseCase.ExecuteAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(Application.DTOs.ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        var result = await _updateProfileUseCase.ExecuteAsync(userId.Value, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Upload profile photo
    /// </summary>
    [HttpPost("photo")]
    [ProducesResponseType(typeof(Application.DTOs.ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new ErrorResponse { Errors = new List<string> { "Invalid token" } });
        }

        if (photo == null || photo.Length == 0)
        {
            return BadRequest(new ErrorResponse { Errors = new List<string> { "Photo file is required" } });
        }

        using var memoryStream = new MemoryStream();
        await photo.CopyToAsync(memoryStream, cancellationToken);

        var request = new UploadPhotoRequest
        {
            PhotoData = memoryStream.ToArray(),
            FileName = photo.FileName
        };

        var result = await _uploadPhotoUseCase.ExecuteAsync(userId.Value, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}