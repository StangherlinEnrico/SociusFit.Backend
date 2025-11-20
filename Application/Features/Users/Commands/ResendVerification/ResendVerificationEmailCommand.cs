using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.ResendVerification;

/// <summary>
/// Command to resend verification email
/// </summary>
public record ResendVerificationEmailCommand : IRequest<Result<string>>
{
    public string Email { get; init; } = string.Empty;
}
