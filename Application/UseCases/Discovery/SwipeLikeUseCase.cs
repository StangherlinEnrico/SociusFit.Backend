using Application.Requests;
using Application.Responses;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Discovery;

public class SwipeLikeUseCase
{
    private readonly ILikeRepository _likeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SwipeLikeUseCase> _logger;

    public SwipeLikeUseCase(
        ILikeRepository likeRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<SwipeLikeUseCase> logger)
    {
        _likeRepository = likeRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<SwipeLikeResponse>> ExecuteAsync(
        Guid currentUserId,
        SwipeLikeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == request.LikedUserId)
        {
            return Result<SwipeLikeResponse>.Failure("Cannot like yourself");
        }

        var likedUser = await _userRepository.GetByIdAsync(request.LikedUserId, cancellationToken);
        if (likedUser == null)
        {
            return Result<SwipeLikeResponse>.Failure("Liked user not found");
        }

        var existingLike = await _likeRepository.ExistsAsync(currentUserId, request.LikedUserId, cancellationToken);
        if (existingLike)
        {
            _logger.LogWarning("User {UserId} already liked {LikedUserId}", currentUserId, request.LikedUserId);
            return Result<SwipeLikeResponse>.Failure("Already liked this user");
        }

        var like = new Like(currentUserId, request.LikedUserId);
        await _likeRepository.CreateAsync(like, cancellationToken);

        var hasMutualLike = await _likeRepository.HasMutualLikeAsync(currentUserId, request.LikedUserId, cancellationToken);

        if (hasMutualLike)
        {
            var existingMatch = await _matchRepository.ExistsAsync(currentUserId, request.LikedUserId, cancellationToken);
            if (!existingMatch)
            {
                var match = new Match(currentUserId, request.LikedUserId);
                await _matchRepository.CreateAsync(match, cancellationToken);

                _logger.LogInformation(
                    "Match created between {User1Id} and {User2Id}",
                    currentUserId,
                    request.LikedUserId);

                var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
                if (currentUser != null)
                {
                    await _notificationService.SendMatchNotificationAsync(
                        request.LikedUserId,
                        currentUser.FirstName,
                        match.Id,
                        cancellationToken);
                }

                return Result<SwipeLikeResponse>.Success(new SwipeLikeResponse
                {
                    IsMatch = true,
                    MatchId = match.Id,
                    MatchedUserName = likedUser.FirstName
                });
            }
        }

        return Result<SwipeLikeResponse>.Success(new SwipeLikeResponse
        {
            IsMatch = false
        });
    }
}