using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Events;
using Domain.Repositories;
using Domain.Validators;
using Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class UploadProfilePhotoUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoStorageRepository _photoStorageRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ICacheService _cacheService;
    private readonly PhotoValidator _photoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<UploadProfilePhotoUseCase> _logger;

    public UploadProfilePhotoUseCase(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IPhotoStorageRepository photoStorageRepository,
        IDomainEventDispatcher eventDispatcher,
        ICacheService cacheService,
        PhotoValidator photoValidator,
        IMapper mapper,
        ILogger<UploadProfilePhotoUseCase> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _photoStorageRepository = photoStorageRepository;
        _eventDispatcher = eventDispatcher;
        _cacheService = cacheService;
        _photoValidator = photoValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProfileDto>> ExecuteAsync(
        Guid userId,
        UploadPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);
            if (profile == null)
            {
                return Result<ProfileDto>.Failure("Profile not found");
            }

            var validationResult = _photoValidator.Validate(request.PhotoData, request.FileName);
            if (!validationResult.IsSuccess)
            {
                return Result<ProfileDto>.Failure(validationResult.Errors);
            }

            var oldPhotoUrl = profile.PhotoUrl;

            var photoUrl = await _photoStorageRepository.UploadPhotoAsync(
                userId,
                request.PhotoData,
                request.FileName,
                cancellationToken
            );

            profile.SetPhotoUrl(photoUrl);
            var updatedProfile = await _profileRepository.UpdateAsync(profile, cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldPhotoUrl))
            {
                try
                {
                    await _photoStorageRepository.DeletePhotoAsync(oldPhotoUrl, cancellationToken);
                    await _eventDispatcher.DispatchAsync(
                        new PhotoDeletedEvent(userId, profile.Id, oldPhotoUrl),
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old photo: {PhotoUrl}", oldPhotoUrl);
                }
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null && !user.ProfileComplete && profile.IsComplete())
            {
                user.MarkProfileComplete();
                await _userRepository.UpdateAsync(user, cancellationToken);

                await _eventDispatcher.DispatchAsync(
                    new ProfileCompletedEvent(profile.Id, userId),
                    cancellationToken
                );
            }

            await _eventDispatcher.DispatchAsync(
                new PhotoUploadedEvent(userId, profile.Id, photoUrl),
                cancellationToken
            );

            var cacheKey = $"profile:user:{userId}";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            var profileDto = _mapper.Map<ProfileDto>(updatedProfile);

            _logger.LogInformation("Photo uploaded successfully for profile: {ProfileId}", profile.Id);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to upload photo: {ex.Message}");
        }
    }
}