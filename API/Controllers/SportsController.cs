using Application.DTOs.Sports;
using Application.DTOs.Users;
using Application.Features.Levels.Queries.GetAll;
using Application.Features.Sports.Commands.Create;
using Application.Features.Sports.Queries.GetAll;
using Application.Features.Sports.Queries.GetPopular;
using Application.Features.UserSports.Commands.Add;
using Application.Features.UserSports.Commands.Remove;
using Application.Features.UserSports.Commands.UpdateLevel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Sports management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SportsController> _logger;

    public SportsController(IMediator mediator, ILogger<SportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all sports
    /// </summary>
    /// <returns>List of all sports</returns>
    /// <response code="200">Sports retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(Application.Common.Models.Result<List<SportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSports()
    {
        var query = new GetAllSportsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get popular sports
    /// </summary>
    /// <param name="count">Number of sports to return (default: 10)</param>
    /// <returns>List of popular sports</returns>
    /// <response code="200">Popular sports retrieved successfully</response>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<List<SportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularSports([FromQuery] int count = 10)
    {
        var query = new GetPopularSportsQuery { Count = count };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new sport
    /// </summary>
    /// <param name="dto">Sport creation data</param>
    /// <returns>Created sport</returns>
    /// <response code="200">Sport created successfully</response>
    /// <response code="400">Sport already exists or invalid input</response>
    [HttpPost]
    [ProducesResponseType(typeof(Application.Common.Models.Result<SportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSport([FromBody] CreateSportDto dto)
    {
        var command = new CreateSportCommand { Name = dto.Name };
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        _logger.LogInformation("Sport created: {SportName}", dto.Name);
        return Ok(result);
    }

    /// <summary>
    /// Get all skill levels
    /// </summary>
    /// <returns>List of all levels</returns>
    /// <response code="200">Levels retrieved successfully</response>
    [HttpGet("levels")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<List<LevelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLevels()
    {
        var query = new GetAllLevelsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

/// <summary>
/// User sports management endpoints
/// </summary>
[ApiController]
[Route("api/v1/users/{userId}/sports")]
[Produces("application/json")]
public class UserSportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserSportsController> _logger;

    public UserSportsController(IMediator mediator, ILogger<UserSportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Add a sport to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">Sport to add</param>
    /// <returns>Added user sport</returns>
    /// <response code="200">Sport added successfully</response>
    /// <response code="400">Invalid input or user already has this sport</response>
    /// <response code="404">User, sport, or level not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserSportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSport(int userId, [FromBody] AddUserSportDto dto)
    {
        var command = new AddUserSportCommand
        {
            UserId = userId,
            SportId = dto.SportId,
            LevelId = dto.LevelId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            // Determine appropriate status code based on error message
            if (result.Message?.Contains("not found") == true)
                return NotFound(result);

            return BadRequest(result);
        }

        _logger.LogInformation("Sport added to user: UserId={UserId}, SportId={SportId}", userId, dto.SportId);
        return Ok(result);
    }

    /// <summary>
    /// Update user's sport level
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="sportId">Sport ID</param>
    /// <param name="dto">New level</param>
    /// <returns>Updated user sport</returns>
    /// <response code="200">Level updated successfully</response>
    /// <response code="404">User sport or level not found</response>
    [HttpPut("{sportId}/level")]
    [ProducesResponseType(typeof(Application.Common.Models.Result<UserSportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLevel(int userId, int sportId, [FromBody] UpdateUserSportDto dto)
    {
        var command = new UpdateUserSportLevelCommand
        {
            UserId = userId,
            SportId = sportId,
            NewLevelId = dto.LevelId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Sport level updated: UserId={UserId}, SportId={SportId}, NewLevelId={LevelId}",
            userId, sportId, dto.LevelId);
        return Ok(result);
    }

    /// <summary>
    /// Remove a sport from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="sportId">Sport ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">Sport removed successfully</response>
    /// <response code="404">User sport not found</response>
    [HttpDelete("{sportId}")]
    [ProducesResponseType(typeof(Application.Common.Models.Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSport(int userId, int sportId)
    {
        var command = new RemoveUserSportCommand
        {
            UserId = userId,
            SportId = sportId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return NotFound(result);

        _logger.LogInformation("Sport removed from user: UserId={UserId}, SportId={SportId}", userId, sportId);
        return Ok(result);
    }
}