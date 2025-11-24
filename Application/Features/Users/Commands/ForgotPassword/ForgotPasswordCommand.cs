using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.ForgotPassword;

/// <summary>
/// Command to request password reset
/// </summary>
public record ForgotPasswordCommand : IRequest<Result<string>>
{
    public required string Email { get; init; }
}