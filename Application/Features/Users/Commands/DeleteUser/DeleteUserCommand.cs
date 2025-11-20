using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Command to soft delete a user
/// </summary>
public record DeleteUserCommand : IRequest<Result>
{
    public int UserId { get; init; }
}
