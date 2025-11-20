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
    IEmailService emailService) : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
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

        // Send verification email
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
            // In production, you might want to queue this for retry
        }

        // Create session (allow login even before email verification)
        var sessionToken = _tokenGenerator.GenerateToken();
        var sessionExpiresAt = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return response
        var response = new AuthResponseDto
        {
            Token = sessionToken,
            ExpiresAt = sessionExpiresAt,
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

        return Result<AuthResponseDto>.SuccessResult(response, "User registered successfully. Please verify your email.");
    }
}
