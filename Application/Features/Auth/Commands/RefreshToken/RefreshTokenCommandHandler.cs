using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand
/// </summary>
public class RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<Result<AuthResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid or expired refresh token");
        }

        var user = refreshToken.User;

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName);

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenExpiration = _jwtService.GetRefreshTokenExpiration();

        // Revoke old refresh token and create new one
        refreshToken.Revoke(request.IpAddress, newRefreshToken);

        var refreshTokenEntity = new Domain.Entities.RefreshToken(
            user.Id,
            newRefreshToken,
            newRefreshTokenExpiration,
            request.IpAddress);

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return response
        var response = new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiration(),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified(),
                Provider = user.Provider,
                Location = user.Location,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                MaxDistanceKm = user.MaxDistanceKm,
                CreatedAt = user.CreatedAt
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Token refreshed successfully");
    }
}