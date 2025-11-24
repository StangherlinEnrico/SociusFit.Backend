using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.Login;

/// <summary>
/// Handler for LoginCommand
/// </summary>
public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
        }

        // Check if user registered with OAuth only
        if (user.HasOAuthProvider() && !user.HasPassword())
        {
            return Result<AuthResponseDto>.FailureResult(
                $"This account uses {user.Provider} sign-in. Please login with {user.Provider}.");
        }

        // Verify password
        if (!user.HasPassword() || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash!))
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
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
            },
            Message = !user.IsEmailVerified()
                ? "Login successful. Please verify your email to access all features."
                : null
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
    }
}