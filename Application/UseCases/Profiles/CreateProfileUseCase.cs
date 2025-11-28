using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Validators;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profiles;

public class CreateProfileUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ProfileValidator _profileValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProfileUseCase> _logger;

    public CreateProfileUseCase(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher,
        ProfileValidator profileValidator,
        IMapper mapper,
        ILogger<CreateProfileUseCase> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
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

            var profile = new Domain.Entities.Profile(
                userId,
                request.Age,
                request.Gender,
                request.City,
                request.Bio
            );

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

            var savedProfile = await _profileRepository.CreateAsync(profile, cancellationToken);

            await _eventDispatcher.DispatchAsync(
                new ProfileCreatedEvent(savedProfile.Id, userId, request.Age, request.Gender, request.City),
                cancellationToken
            );

            var profileDto = _mapper.Map<ProfileDto>(savedProfile);

            _logger.LogInformation("Profile created successfully: {ProfileId} for user {UserId}", savedProfile.Id, userId);

            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile for user: {UserId}", userId);
            return Result<ProfileDto>.Failure($"Failed to create profile: {ex.Message}");
        }
    }
}