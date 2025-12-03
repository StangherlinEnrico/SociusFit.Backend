using Application.Requests;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Notifications;

public class RegisterDeviceTokenUseCase
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly ILogger<RegisterDeviceTokenUseCase> _logger;

    public RegisterDeviceTokenUseCase(
        IDeviceTokenRepository deviceTokenRepository,
        ILogger<RegisterDeviceTokenUseCase> logger)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> ExecuteAsync(
        Guid userId,
        RegisterDeviceTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Result<bool>.Failure("Device token cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(request.Platform))
        {
            return Result<bool>.Failure("Platform cannot be empty");
        }

        if (request.Platform.ToLower() != "android" && request.Platform.ToLower() != "ios")
        {
            return Result<bool>.Failure("Platform must be either 'android' or 'ios'");
        }

        var existingToken = await _deviceTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (existingToken != null)
        {
            if (existingToken.UserId != userId)
            {
                await _deviceTokenRepository.DeactivateAllByUserIdAsync(existingToken.UserId, cancellationToken);
            }

            existingToken.UpdateToken(request.Token);
            await _deviceTokenRepository.UpdateAsync(existingToken, cancellationToken);

            _logger.LogInformation("Device token updated for user {UserId}", userId);
        }
        else
        {
            await _deviceTokenRepository.DeactivateAllByUserIdAsync(userId, cancellationToken);

            var deviceToken = new DeviceToken(userId, request.Token, request.Platform.ToLower());
            await _deviceTokenRepository.CreateAsync(deviceToken, cancellationToken);

            _logger.LogInformation("New device token registered for user {UserId}", userId);
        }

        return Result<bool>.Success(true);
    }
}