using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class GetProfileByUserIdUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProfileByUserIdUseCase> _logger;

    public GetProfileByUserIdUseCase(
        IProfileRepository profileRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<GetProfileByUserIdUseCase> logger)
    {
        _profileRepository = profileRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProfileDto>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"profile:user:{userId}";

            var cachedProfile = await _cacheService.GetAsync<ProfileDto>(cacheKey, cancellationToken);
            if (cachedProfile != null)
            {
                _logger.LogDebug("Profile retrieved from cache for user: {UserId}", userId);
                return Result<ProfileDto>.Success(cachedProfile);
            }

            var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);
            if (profile == null)
            {
                _logger.LogWarning("Profile not found for user: {UserId}", userId);
                return Result<ProfileDto>.Failure("Profile not found");
            }

            var profileDto = _mapper.Map<ProfileDto>(profile);

            await _cacheService.SetAsync(cacheKey, profileDto, TimeSpan.FromMinutes(30), cancellationToken);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to get profile: {ex.Message}");
        }
    }
}