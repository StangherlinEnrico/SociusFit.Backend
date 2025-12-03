using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Chat;

public class GetMessagesUseCase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMessagesUseCase> _logger;

    public GetMessagesUseCase(
        IMessageRepository messageRepository,
        IMatchRepository matchRepository,
        IMapper mapper,
        ILogger<GetMessagesUseCase> logger)
    {
        _messageRepository = messageRepository;
        _matchRepository = matchRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<MessageDto>>> ExecuteAsync(
        Guid matchId,
        Guid userId,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var match = await _matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            return Result<List<MessageDto>>.Failure("Match not found");
        }

        if (match.User1Id != userId && match.User2Id != userId)
        {
            return Result<List<MessageDto>>.Failure("User is not part of this match");
        }

        var messages = await _messageRepository.GetByMatchIdAsync(
            matchId,
            Math.Min(pageSize, 100),
            Math.Max(pageNumber, 1),
            cancellationToken);

        var messageDtos = _mapper.Map<List<MessageDto>>(messages);
        return Result<List<MessageDto>>.Success(messageDtos);
    }
}