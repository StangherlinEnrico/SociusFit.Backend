using Application.DTOs;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Matches;

public class GetMatchesUseCase
{
    private readonly IMatchRepository _matchRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISportRepository _sportRepository;
    private readonly ILogger<GetMatchesUseCase> _logger;

    public GetMatchesUseCase(
        IMatchRepository matchRepository,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ISportRepository sportRepository,
        ILogger<GetMatchesUseCase> logger)
    {
        _matchRepository = matchRepository;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _sportRepository = sportRepository;
        _logger = logger;
    }

    public async Task<Result<List<MatchDto>>> ExecuteAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var matches = await _matchRepository.GetUserMatchesAsync(currentUserId, cancellationToken);
        var matchDtos = new List<MatchDto>();

        var currentProfile = await _profileRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentProfile == null)
        {
            return Result<List<MatchDto>>.Failure("Profile not found");
        }

        var sports = await _sportRepository.GetAllAsync(cancellationToken);
        var sportsMap = sports.ToDictionary(s => s.Id, s => s.Name);

        foreach (var match in matches)
        {
            var otherUserId = match.GetOtherUserId(currentUserId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId, cancellationToken);
            var otherProfile = await _profileRepository.GetByUserIdAsync(otherUserId, cancellationToken);

            if (otherUser == null || otherProfile == null)
            {
                _logger.LogWarning("Missing user or profile for match {MatchId}", match.Id);
                continue;
            }

            var currentSportIds = currentProfile.ProfileSports.Select(ps => ps.SportId).ToHashSet();
            var otherSportIds = otherProfile.ProfileSports.Select(ps => ps.SportId).ToHashSet();
            var commonSportIds = currentSportIds.Intersect(otherSportIds).ToList();

            var commonSports = commonSportIds
                .Select(sportId => new SportInfoDto
                {
                    SportId = sportId,
                    SportName = sportsMap.GetValueOrDefault(sportId, "Unknown"),
                    Level = otherProfile.ProfileSports
                        .First(ps => ps.SportId == sportId)
                        .Level.ToString()
                })
                .ToList();

            matchDtos.Add(new MatchDto
            {
                MatchId = match.Id,
                OtherUserId = otherUserId,
                OtherUserName = otherUser.FirstName,
                OtherUserAge = otherProfile.Age,
                OtherUserCity = otherProfile.City,
                OtherUserPhotoUrl = otherProfile.PhotoUrl,
                CommonSports = commonSports,
                MatchedAt = match.CreatedAt
            });
        }

        return Result<List<MatchDto>>.Success(matchDtos);
    }
}