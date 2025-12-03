using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Chat;

public class MarkMessagesAsReadUseCase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ILogger<MarkMessagesAsReadUseCase> _logger;

    public MarkMessagesAsReadUseCase(
        IMessageRepository messageRepository,
        IMatchRepository matchRepository,
        ILogger<MarkMessagesAsReadUseCase> logger)
    {
        _messageRepository = messageRepository;
        _matchRepository = matchRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> ExecuteAsync(
        Guid matchId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var match = await _matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            return Result<bool>.Failure("Match not found");
        }

        if (match.User1Id != userId && match.User2Id != userId)
        {
            return Result<bool>.Failure("User is not part of this match");
        }

        await _messageRepository.MarkMessagesAsReadAsync(matchId, userId, cancellationToken);

        _logger.LogInformation(
            "Messages marked as read in match {MatchId} by user {UserId}",
            matchId,
            userId);

        return Result<bool>.Success(true);
    }
}