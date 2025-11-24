using Application.Common.Models;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to logout user (revoke all refresh tokens)
/// </summary>
public record LogoutCommand : IRequest<Result>
{
    public required int UserId { get; init; }
    public string? IpAddress { get; init; }

    /// <summary>
    /// If true, revokes all tokens. If false, revokes only the provided refresh token.
    /// </summary>
    public bool RevokeAll { get; init; } = true;

    /// <summary>
    /// Specific refresh token to revoke (used when RevokeAll is false)
    /// </summary>
    public string? RefreshToken { get; init; }
}