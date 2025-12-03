using Application.DTOs;
using Application.Requests;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Discovery;

public class GetDiscoveryCardsUseCase
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ISportRepository _sportRepository;
    private readonly ILogger<GetDiscoveryCardsUseCase> _logger;

    public GetDiscoveryCardsUseCase(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ILikeRepository likeRepository,
        IMatchRepository matchRepository,
        ISportRepository sportRepository,
        ILogger<GetDiscoveryCardsUseCase> logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _likeRepository = likeRepository;
        _matchRepository = matchRepository;
        _sportRepository = sportRepository;
        _logger = logger;
    }

    public async Task<Result<List<DiscoveryCardDto>>> ExecuteAsync(
        Guid currentUserId,
        GetDiscoveryCardsRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentProfile = await _profileRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentProfile == null)
        {
            return Result<List<DiscoveryCardDto>>.Failure("Profile not found");
        }

        var likedUserIds = await _likeRepository.GetLikedUserIdsAsync(currentUserId, cancellationToken);
        var matchedUserIds = await _matchRepository.GetMatchedUserIdsAsync(currentUserId, cancellationToken);
        var excludedUserIds = likedUserIds.Concat(matchedUserIds).Append(currentUserId).ToHashSet();

        var allProfiles = await GetAllProfilesAsync(cancellationToken);
        var users = await GetUsersMapAsync(allProfiles.Select(p => p.UserId), cancellationToken);
        var sports = await _sportRepository.GetAllAsync(cancellationToken);
        var sportsMap = sports.ToDictionary(s => s.Id, s => s.Name);

        var compatibleCards = allProfiles
            .Where(p => !excludedUserIds.Contains(p.UserId))
            .Select(p => BuildDiscoveryCard(p, users[p.UserId], currentProfile, sportsMap, request.SportId))
            .Where(card => card != null && IsCompatible(card, currentProfile, request.SportId))
            .OrderBy(card => card!.DistanceKm)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Result<List<DiscoveryCardDto>>.Success(compatibleCards!);
    }

    private async Task<List<Domain.Entities.Profile>> GetAllProfilesAsync(CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var profiles = new List<Domain.Entities.Profile>();

        foreach (var user in allUsers)
        {
            var profile = await _profileRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (profile != null)
            {
                profiles.Add(profile);
            }
        }

        return profiles;
    }

    private async Task<Dictionary<Guid, Domain.Entities.User>> GetUsersMapAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var users = new Dictionary<Guid, Domain.Entities.User>();
        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                users[userId] = user;
            }
        }
        return users;
    }

    private DiscoveryCardDto? BuildDiscoveryCard(
        Domain.Entities.Profile profile,
        Domain.Entities.User user,
        Domain.Entities.Profile currentProfile,
        Dictionary<Guid, string> sportsMap,
        Guid? filterSportId)
    {
        var sports = profile.ProfileSports
            .Where(ps => !filterSportId.HasValue || ps.SportId == filterSportId.Value)
            .Select(ps => new SportInfoDto
            {
                SportId = ps.SportId,
                SportName = sportsMap.GetValueOrDefault(ps.SportId, "Unknown"),
                Level = ps.Level.ToString()
            })
            .ToList();

        if (!sports.Any())
        {
            return null;
        }

        var distanceKm = CalculateDistance(
            currentProfile.Latitude,
            currentProfile.Longitude,
            profile.Latitude,
            profile.Longitude);

        return new DiscoveryCardDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            Age = profile.Age,
            City = profile.City,
            PhotoUrl = profile.PhotoUrl,
            Bio = profile.Bio,
            DistanceKm = Math.Round(distanceKm, 1),
            Sports = sports
        };
    }

    private bool IsCompatible(
        DiscoveryCardDto card,
        Domain.Entities.Profile currentProfile,
        Guid? filterSportId)
    {
        if (card.DistanceKm > currentProfile.MaxDistance)
        {
            return false;
        }

        if (filterSportId.HasValue)
        {
            var currentSport = currentProfile.ProfileSports
                .FirstOrDefault(ps => ps.SportId == filterSportId.Value);

            if (currentSport == null)
            {
                return false;
            }

            var targetSport = card.Sports.FirstOrDefault(s => s.SportId == filterSportId.Value);
            if (targetSport == null)
            {
                return false;
            }

            var currentLevel = (int)currentSport.Level;
            var targetLevel = (int)Enum.Parse<SportLevel>(targetSport.Level);
            var levelDiff = Math.Abs(currentLevel - targetLevel);

            if (levelDiff > 1)
            {
                return false;
            }
        }
        else
        {
            var currentSportIds = currentProfile.ProfileSports.Select(ps => ps.SportId).ToHashSet();
            var targetSportIds = card.Sports.Select(s => s.SportId).ToHashSet();

            if (!currentSportIds.Overlaps(targetSportIds))
            {
                return false;
            }
        }

        return true;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}