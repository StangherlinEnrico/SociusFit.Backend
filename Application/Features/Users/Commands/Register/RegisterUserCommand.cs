using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.Register;

/// <summary>
/// Command to register a new user
/// </summary>
public record RegisterUserCommand : IRequest<Result<AuthResponseDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Handler for RegisterUserCommand
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

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

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
                Location = user.Location,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                MaxDistanceKm = user.MaxDistanceKm,
                CreatedAt = user.CreatedAt
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "User registered successfully");
    }
}