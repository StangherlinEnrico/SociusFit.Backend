using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.VerifyEmail;

/// <summary>
/// Command to verify user email
/// </summary>
public record VerifyEmailCommand : IRequest<Result<string>>
{
    public string Token { get; init; } = string.Empty;
}
