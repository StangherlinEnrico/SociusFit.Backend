using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handler for LogoutCommand
/// </summary>
public class LogoutCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = request.IpAddress ?? "unknown";

        if (request.RevokeAll)
        {
            // Revoke all user tokens
            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(
                request.UserId,
                ipAddress,
                cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            // Revoke specific token
            var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(
                request.RefreshToken,
                cancellationToken);

            if (token == null || token.UserId != request.UserId)
            {
                return Result.FailureResult("Invalid refresh token");
            }

            if (token.IsActive)
            {
                token.Revoke(ipAddress);
            }
        }
        else
        {
            return Result.FailureResult("Refresh token is required when not revoking all tokens");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Logged out successfully");
    }
}