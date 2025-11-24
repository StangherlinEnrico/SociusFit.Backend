using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.ResetPassword;

/// <summary>
/// Command to reset password using token
/// </summary>
public record ResetPasswordCommand : IRequest<Result<string>>
{
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}