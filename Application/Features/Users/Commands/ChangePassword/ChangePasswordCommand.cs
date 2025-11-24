using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.ChangePassword;

/// <summary>
/// Command to change password for authenticated user
/// </summary>
public record ChangePasswordCommand : IRequest<Result<string>>
{
    public required int UserId { get; init; }
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
    public string? IpAddress { get; init; }
}