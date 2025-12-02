using Domain.Common;
using Infrastructure.Authentication;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users;

public class LogoutUserUseCase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<LogoutUserUseCase> _logger;

    public LogoutUserUseCase(
        ITokenService tokenService,
        ILogger<LogoutUserUseCase> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<bool>> ExecuteAsync(
        string token,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify token belongs to user
            var tokenUserId = _tokenService.GetUserId(token);

            if (tokenUserId == null || tokenUserId.Value != userId)
            {
                _logger.LogWarning(
                    "Logout attempt with mismatched user ID. Token user: {TokenUserId}, Request user: {UserId}",
                    tokenUserId,
                    userId
                );
                return Result<bool>.Failure("Invalid token for this user");
            }

            // Revoke the token
            await _tokenService.RevokeTokenAsync(token, "User logout", cancellationToken);

            _logger.LogInformation("User {UserId} logged out successfully", userId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            return Result<bool>.Failure($"Logout failed: {ex.Message}");
        }
    }
}