using Application.Common.Models;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

/// <summary>
/// Command to update user profile
/// </summary>
public record UpdateUserProfileCommand : IRequest<Result<UserDto>>
{
    public int UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
