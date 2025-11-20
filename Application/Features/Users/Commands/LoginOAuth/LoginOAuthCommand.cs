using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.LoginOAuth;

/// <summary>
/// Command to login with OAuth provider
/// </summary>
public record LoginOAuthCommand : IRequest<Result<AuthResponseDto>>
{
    public string Provider { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}

/// <summary>
/// Handler for LoginOAuthCommand
/// </summary>
public class LoginOAuthCommandHandler : IRequestHandler<LoginOAuthCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOAuthService _oAuthService;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginOAuthCommandHandler(
        IUnitOfWork unitOfWork,
        IOAuthService oAuthService,
        ITokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _oAuthService = oAuthService;
        _tokenGenerator = tokenGenerator;
    }

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
            user.VerifyEmail();

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

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
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "OAuth login successful");
    }
}