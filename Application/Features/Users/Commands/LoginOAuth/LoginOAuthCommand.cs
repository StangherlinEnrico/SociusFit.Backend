using Application.Common.Models;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands.LoginOAuth;

/// <summary>
/// Command to login with OAuth provider
/// </summary>
public record LoginOAuthCommand : IRequest<Result<AuthResponseDto>>
{
    public string Provider { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}
