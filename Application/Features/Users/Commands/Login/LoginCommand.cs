using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.Login;

/// <summary>
/// Command to login with email and password
/// </summary>
public record LoginCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Handler for LoginCommand
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

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

        // Create session
        var token = _tokenGenerator.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var session = new Session(user.Id, token, expiresAt);

        await _unitOfWork.Sessions.AddAsync(session, cancellationToken);
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
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
    }
}
