using Application.Common.Models;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken;

/// <summary>
/// Command to revoke a refresh token (logout)
/// </summary>
public record RevokeTokenCommand : IRequest<Result>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}
