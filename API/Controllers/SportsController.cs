using Application.DTOs;
using Application.UseCases.Sports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SportsController : ControllerBase
{
    private readonly GetAllSportsUseCase _getAllSportsUseCase;
    private readonly ILogger<SportsController> _logger;

    public SportsController(
        GetAllSportsUseCase getAllSportsUseCase,
        ILogger<SportsController> logger)
    {
        _getAllSportsUseCase = getAllSportsUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Get all available sports
    /// </summary>
    /// <remarks>
    /// Returns the complete list of sports available in the system.
    /// Mobile app should call this endpoint to display sport selection during profile creation/update.
    /// Each sport has an ID that must be used when creating/updating profile with AddSportRequest.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(List<SportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getAllSportsUseCase.ExecuteAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to retrieve sports: {Errors}", string.Join(", ", result.Errors));
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse { Errors = result.Errors.ToList() });
        }

        return Ok(result.Value);
    }
}