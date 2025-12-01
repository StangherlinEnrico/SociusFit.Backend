using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Events;
using Domain.Repositories;
using Domain.Validators;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class UpdateProfileUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ProfileValidator _profileValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProfileUseCase> _logger;

    public UpdateProfileUseCase(
        IProfileRepository profileRepository,
        ISportRepository sportRepository,
        IDomainEventDispatcher eventDispatcher,
        ProfileValidator profileValidator,
        IMapper mapper,
        ILogger<UpdateProfileUseCase> logger)
    {
        _profileRepository = profileRepository;
        _sportRepository = sportRepository;
        _eventDispatcher = eventDispatcher;
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

            // Aggiorna le info base
            profile.UpdateBasicInfo(request.Age, request.Gender, request.City, request.Bio);

            // Aggiorna la distanza massima
            profile.UpdateMaxDistance(request.MaxDistance);

            // Rimuovi tutti gli sport e riaggiungili
            profile.ClearSports();

            foreach (var sportRequest in request.Sports)
            {
                // Verifica che lo sport esista
                var sport = await _sportRepository.GetByIdAsync(sportRequest.SportId, cancellationToken);
                if (sport == null)
                {
                    return Result<ProfileDto>.Failure($"Sport with ID {sportRequest.SportId} not found");
                }

                profile.AddSport(sportRequest.SportId, sportRequest.Level);
            }

            // Valida il profilo aggiornato
            var validationResult = _profileValidator.Validate(profile);
            if (!validationResult.IsSuccess)
            {
                return Result<ProfileDto>.Failure(validationResult.Errors);
            }

            // Salva le modifiche
            var updatedProfile = await _profileRepository.UpdateAsync(profile, cancellationToken);

            // Dispatch event
            await _eventDispatcher.DispatchAsync(
                new ProfileUpdatedEvent(profile.Id, userId),
                cancellationToken
            );

            var profileDto = _mapper.Map<ProfileDto>(updatedProfile);

            _logger.LogInformation("Profile updated successfully: {ProfileId} for user {UserId}", profile.Id, userId);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to update profile: {ex.Message}");
        }
    }
}