using Application.Common.Models;
using Application.Features.Auth.Commands.RevokeToken;
using Domain.Interfaces;
using MediatR;

/// <summary>
/// Handler for RevokeTokenCommand
/// </summary>
public class RevokeTokenCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RevokeTokenCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return Result.FailureResult("Invalid refresh token");
        }

        // Revoke token
        refreshToken.Revoke(request.IpAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Token revoked successfully");
    }
}