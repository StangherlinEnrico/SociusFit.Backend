using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.Register;

/// <summary>
/// Handler for RegisterUserCommand
/// </summary>
public class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    IJwtService jwtService,
    IEmailService emailService) : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result<AuthResponseDto>.FailureResult("Email already exists");
        }

        // Create user
        var user = new User(request.FirstName, request.LastName, request.Email);
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        user.SetPassword(passwordHash);

        // Generate email verification token (expires in 24 hours)
        var verificationToken = _tokenGenerator.GenerateToken();
        var tokenExpiresAt = DateTime.UtcNow.AddHours(24);
        user.SetEmailVerificationToken(verificationToken, tokenExpiresAt);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        // Send verification email (don't fail if email fails)
        try
        {
            await _emailService.SendVerificationEmailAsync(
                user.Email,
                user.FirstName,
                verificationToken,
                cancellationToken);
        }
        catch (Exception)
        {
            // Log error but don't fail registration
        }

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
                Location = user.Location,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                MaxDistanceKm = user.MaxDistanceKm,
                CreatedAt = user.CreatedAt
            },
            Message = "Registration successful! Please check your email to verify your account."
        };

        return Result<AuthResponseDto>.SuccessResult(
            response,
            "User registered successfully. Please verify your email.");
    }
}