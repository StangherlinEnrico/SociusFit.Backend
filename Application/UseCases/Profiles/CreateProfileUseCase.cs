using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services;
using Domain.Validators;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class CreateProfileUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ProfileValidator _profileValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProfileUseCase> _logger;

    public CreateProfileUseCase(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ISportRepository sportRepository,
        IGeocodingService geocodingService,
        IDomainEventDispatcher eventDispatcher,
        ProfileValidator profileValidator,
        IMapper mapper,
        ILogger<CreateProfileUseCase> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _sportRepository = sportRepository;
        _geocodingService = geocodingService;
        _eventDispatcher = eventDispatcher;
        _profileValidator = profileValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProfileDto>> ExecuteAsync(
        Guid userId,
        CreateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result<ProfileDto>.Failure("User not found");
            }

            var existingProfile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);
            if (existingProfile != null)
            {
                return Result<ProfileDto>.Failure("Profile already exists for this user");
            }

            // GEOCODING: Ottieni coordinate dalla città
            var coordinates = await _geocodingService.GetCoordinatesAsync(request.City, cancellationToken);

            if (coordinates == null)
            {
                return Result<ProfileDto>.Failure($"Unable to geocode city: {request.City}. Please check the city name format.");
            }

            var (latitude, longitude) = coordinates.Value;

            // Crea profilo con coordinate geocodificate
            var profile = new Domain.Entities.Profile(
                userId,
                request.Age,
                request.Gender,
                request.City,
                latitude,
                longitude,
                request.Bio,
                request.MaxDistance
            );

            // Aggiungi sport al profilo
            foreach (var sportRequest in request.Sports)
            {
                var sport = await _sportRepository.GetByIdAsync(sportRequest.SportId, cancellationToken);
                if (sport == null)
                {
                    return Result<ProfileDto>.Failure($"Sport with ID {sportRequest.SportId} not found");
                }

                profile.AddSport(sportRequest.SportId, sportRequest.Level);
            }

            var validationResult = _profileValidator.Validate(profile);
            if (!validationResult.IsSuccess)
            {
                return Result<ProfileDto>.Failure(validationResult.Errors);
            }

            var savedProfile = await _profileRepository.CreateAsync(profile, cancellationToken);

            await _eventDispatcher.DispatchAsync(
                new ProfileCreatedEvent(savedProfile.Id, userId, request.Age, request.Gender, request.City),
                cancellationToken
            );

            var profileDto = _mapper.Map<ProfileDto>(savedProfile);

            _logger.LogInformation("Profile created successfully: {ProfileId} for user {UserId} in {City} (geocoded to {Latitude}, {Longitude})",
                savedProfile.Id, userId, request.City, latitude, longitude);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to create profile: {ex.Message}");
        }
    }
}