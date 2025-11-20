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
    ITokenGenerator tokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

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

        // Verify password
        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
        }

        // OPTIONAL: Require email verification before login
        // Uncomment the following lines if you want to enforce email verification
        /*
        if (!user.IsEmailVerified())
        {
            return Result<AuthResponseDto>.FailureResult(
                "Please verify your email before logging in. Check your inbox for the verification link.");
        }
        */

        // Create session
        var token = _tokenGenerator.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return response
        var response = new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
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
            },
            Message = !user.IsEmailVerified()
                ? "Login successful. Please verify your email to access all features."
                : null
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
    }
}
