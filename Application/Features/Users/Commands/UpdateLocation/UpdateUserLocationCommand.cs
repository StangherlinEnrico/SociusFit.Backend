using Application.Common.Models;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands.UpdateLocation;

/// <summary>
/// Command to update user location settings
/// </summary>
public record UpdateUserLocationCommand : IRequest<Result<UserDto>>
{
    public int UserId { get; init; }
    public string? Location { get; init; }
    public int? MaxDistance { get; init; }
}