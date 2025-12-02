using Application.Requests;
using Application.Validators;
using Domain.Common;
using Domain.Repositories;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users;

/// <summary>
/// Use case for complete account deletion (hard delete)
/// Deletes all user data including:
/// - User entity
/// - UserCredentials (password)
/// - Profile (with cascade to ProfileSports)
/// - Profile photo from blob storage
/// - Revoked tokens
/// </summary>
public class DeleteAccountUseCase
{
    private readonly SociusFitDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IPhotoStorageRepository _photoStorageRepository;
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly DeleteAccountRequestValidator _validator;
    private readonly ILogger<DeleteAccountUseCase> _logger;

    public DeleteAccountUseCase(
        SociusFitDbContext context,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IPhotoStorageRepository photoStorageRepository,
        IRevokedTokenRepository revokedTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        DeleteAccountRequestValidator validator,
        ILogger<DeleteAccountUseCase> logger)
    {
        _context = context;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _photoStorageRepository = photoStorageRepository;
        _revokedTokenRepository = revokedTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<bool>> ExecuteAsync(
        Guid userId,
        DeleteAccountRequest request,
        string currentToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validate request
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation("Starting account deletion for user {UserId}", userId);

            // 2. Get user
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            // 3. Verify password
            var credentials = await _context.UserCredentials
                .FirstOrDefaultAsync(uc => uc.UserId == userId, cancellationToken);

            if (credentials == null)
            {
                return Result<bool>.Failure("User credentials not found");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, credentials.PasswordHash))
            {
                _logger.LogWarning("Failed account deletion attempt for user {UserId}: incorrect password", userId);
                return Result<bool>.Failure("Incorrect password. Account deletion cancelled.");
            }

            _logger.LogInformation("Password verified for user {UserId}. Proceeding with deletion...", userId);

            // 4. Get profile (if exists)
            var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);

            // 5. Delete profile photo from blob storage (if exists)
            if (profile != null && !string.IsNullOrWhiteSpace(profile.PhotoUrl))
            {
                try
                {
                    await _photoStorageRepository.DeletePhotoAsync(profile.PhotoUrl, cancellationToken);
                    _logger.LogInformation("Deleted profile photo for user {UserId}: {PhotoUrl}", userId, profile.PhotoUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete profile photo for user {UserId}: {PhotoUrl}", userId, profile.PhotoUrl);
                    // Continue with deletion even if photo deletion fails
                }
            }

            // 6. Delete profile (cascade deletes ProfileSports automatically)
            if (profile != null)
            {
                await _profileRepository.DeleteAsync(profile.Id, cancellationToken);
                _logger.LogInformation("Deleted profile for user {UserId}", userId);
            }

            // 7. Delete user credentials
            _context.UserCredentials.Remove(credentials);
            _logger.LogInformation("Deleted credentials for user {UserId}", userId);

            // 8. Delete user (this is the main entity)
            await _userRepository.DeleteAsync(userId, cancellationToken);
            _logger.LogInformation("Deleted user entity: {UserId}", userId);

            // 9. Revoke current token
            try
            {
                await _tokenService.RevokeTokenAsync(currentToken, "Account deleted", cancellationToken);
                _logger.LogInformation("Revoked current token for deleted user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke token during account deletion for user {UserId}", userId);
                // Continue even if token revocation fails
            }

            // 10. Delete all revoked tokens for this user (cleanup)
            var revokedTokens = await _revokedTokenRepository.GetByUserIdAsync(userId, cancellationToken);
            foreach (var revokedToken in revokedTokens)
            {
                _context.RevokedTokens.Remove(revokedToken);
            }
            _logger.LogInformation("Deleted {Count} revoked tokens for user {UserId}", revokedTokens.Count, userId);

            // 11. Log deletion reason (if provided)
            if (!string.IsNullOrWhiteSpace(request.Reason))
            {
                _logger.LogInformation("Account deletion reason for user {UserId}: {Reason}", userId, request.Reason);
            }

            _logger.LogInformation(
                "Account successfully deleted for user {UserId} ({Email}). All associated data removed.",
                userId,
                user.Email
            );

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account for user {UserId}", userId);
            return Result<bool>.Failure($"Failed to delete account: {ex.Message}");
        }
    }
}