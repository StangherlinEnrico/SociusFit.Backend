using Application.Common.Models;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands.LoginOAuth;

/// <summary>
/// Command to login with OAuth provider (Google, Apple)
/// </summary>
public record LoginOAuthCommand : IRequest<Result<AuthResponseDto>>
{
    public required string Provider { get; init; }
    public required string Token { get; init; }
    public string? IpAddress { get; init; }

    /// <summary>
    /// First name from OAuth provider (required for Apple first login)
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Last name from OAuth provider (required for Apple first login)
    /// </summary>
    public string? LastName { get; init; }
}