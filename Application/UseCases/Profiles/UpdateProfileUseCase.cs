using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Validators;
using Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class UpdateProfileUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ICacheService _cacheService;
    private readonly ProfileValidator _profileValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProfileUseCase> _logger;

    public UpdateProfileUseCase(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher,
        ICacheService cacheService,
        ProfileValidator profileValidator,
        IMapper mapper,
        ILogger<UpdateProfileUseCase> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
        _cacheService = cacheService;
        _profileValidator = profileValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProfileDto>> ExecuteAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);
            if (profile == null)
            {
                return Result<ProfileDto>.Failure("Profile not found");
            }

            profile.UpdateBasicInfo(request.Age, request.Gender, request.City, request.Bio);

            profile.ClearSports();
            foreach (var sportRequest in request.Sports)
            {
                var sport = new Sport(sportRequest.Name, sportRequest.Level);
                profile.AddSport(sport);
            }

            var validationResult = _profileValidator.Validate(profile);
            if (!validationResult.IsSuccess)
            {
                return Result<ProfileDto>.Failure(validationResult.Errors);
            }

            var updatedProfile = await _profileRepository.UpdateAsync(profile, cancellationToken);

            var wasComplete = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (wasComplete != null && !wasComplete.ProfileComplete && profile.IsComplete())
            {
                wasComplete.MarkProfileComplete();
                await _userRepository.UpdateAsync(wasComplete, cancellationToken);

                await _eventDispatcher.DispatchAsync(
                    new ProfileCompletedEvent(profile.Id, userId),
                    cancellationToken
                );
            }

            await _eventDispatcher.DispatchAsync(
                new ProfileUpdatedEvent(profile.Id, userId),
                cancellationToken
            );

            var cacheKey = $"profile:user:{userId}";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            var profileDto = _mapper.Map<ProfileDto>(updatedProfile);

            _logger.LogInformation("Profile updated successfully: {ProfileId}", profile.Id);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to update profile: {ex.Message}");
        }
    }
}