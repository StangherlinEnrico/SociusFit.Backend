using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.LoginOAuth;

/// <summary>
/// Handler for LoginOAuthCommand
/// </summary>
public class LoginOAuthCommandHandler(
    IUnitOfWork unitOfWork,
    IOAuthService oAuthService,
    IJwtService jwtService) : IRequestHandler<LoginOAuthCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IOAuthService _oAuthService = oAuthService;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<Result<AuthResponseDto>> Handle(
        LoginOAuthCommand request,
        CancellationToken cancellationToken)
    {
        // Validate OAuth token
        var providerData = await _oAuthService.ValidateProviderTokenAsync(
            request.Provider,
            request.Token,
            cancellationToken);

        if (providerData == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid OAuth token");
        }

        // Find or create user
        var user = await _unitOfWork.Users.GetByProviderAsync(
            request.Provider,
            providerData.Value.ProviderId,
            cancellationToken);

        if (user == null)
        {
            // Create new user
            user = new User(
                providerData.Value.FirstName,
                providerData.Value.LastName,
                providerData.Value.Email);
            user.SetOAuthProvider(request.Provider, providerData.Value.ProviderId);
            user.VerifyEmail(); // OAuth users are auto-verified

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Generate JWT tokens
        var accessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName);

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiration = _jwtService.GetRefreshTokenExpiration();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken(
            user.Id,
            refreshToken,
            refreshTokenExpiration,
            request.IpAddress ?? "unknown");

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return response
        var response = new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
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

        return Result<AuthResponseDto>.SuccessResult(response, "OAuth login successful");
    }
}