using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Constants;
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
        // Validate provider
        if (!AuthConstants.OAuthProviders.IsSupported(request.Provider))
        {
            return Result<AuthResponseDto>.FailureResult(
                $"Unsupported OAuth provider. Supported providers: {string.Join(", ", AuthConstants.OAuthProviders.Supported)}");
        }

        // Validate OAuth token
        var userInfo = await _oAuthService.ValidateTokenAsync(
            request.Provider,
            request.Token,
            cancellationToken);

        if (userInfo == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid OAuth token");
        }

        var normalizedProvider = request.Provider.ToLowerInvariant();

        // Find existing user by provider
        var user = await _unitOfWork.Users.GetByProviderAsync(
            normalizedProvider,
            userInfo.ProviderId,
            cancellationToken);

        if (user == null && userInfo.HasEmail)
        {
            // Check if user exists with same email (link accounts)
            user = await _unitOfWork.Users.GetByEmailAsync(userInfo.Email, cancellationToken);

            if (user != null)
            {
                // Link OAuth provider to existing account
                user.SetOAuthProvider(normalizedProvider, userInfo.ProviderId);
                if (!user.IsEmailVerified())
                {
                    user.VerifyEmail();
                }
            }
        }

        if (user == null)
        {
            // Create new user
            // For Apple, use provided names from request if OAuth didn't return them
            var firstName = !string.IsNullOrWhiteSpace(userInfo.FirstName)
                ? userInfo.FirstName
                : request.FirstName ?? "User";

            var lastName = !string.IsNullOrWhiteSpace(userInfo.LastName)
                ? userInfo.LastName
                : request.LastName ?? "";

            // Email is required for new users
            if (!userInfo.HasEmail)
            {
                return Result<AuthResponseDto>.FailureResult(
                    "Email is required for registration. Please allow email sharing with the app.");
            }

            user = User.CreateFromOAuth(
                firstName,
                lastName,
                userInfo.Email,
                normalizedProvider,
                userInfo.ProviderId);

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Record login
        user.RecordLogin();
        _unitOfWork.Users.Update(user);

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
                CreatedAt = user.CreatedAt
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "OAuth login successful");
    }
}