using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Chat;

public class SendMessageUseCase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<SendMessageUseCase> _logger;

    public SendMessageUseCase(
        IMessageRepository messageRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<SendMessageUseCase> logger)
    {
        _messageRepository = messageRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<MessageDto>> ExecuteAsync(
        Guid matchId,
        Guid senderId,
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<MessageDto>.Failure("Message content cannot be empty");
        }

        if (request.Content.Length > 2000)
        {
            return Result<MessageDto>.Failure("Message content cannot exceed 2000 characters");
        }

        var match = await _matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            return Result<MessageDto>.Failure("Match not found");
        }

        if (match.User1Id != senderId && match.User2Id != senderId)
        {
            return Result<MessageDto>.Failure("User is not part of this match");
        }

        var message = new Message(matchId, senderId, request.Content.Trim());
        var createdMessage = await _messageRepository.CreateAsync(message, cancellationToken);

        _logger.LogInformation(
            "Message sent in match {MatchId} by user {SenderId}",
            matchId,
            senderId);

        var receiverId = match.GetOtherUserId(senderId);
        var sender = await _userRepository.GetByIdAsync(senderId, cancellationToken);

        if (sender != null)
        {
            await _notificationService.SendMessageNotificationAsync(
                receiverId,
                sender.FirstName,
                matchId,
                request.Content,
                cancellationToken);
        }

        var messageDto = _mapper.Map<MessageDto>(createdMessage);
        return Result<MessageDto>.Success(messageDto);
    }
}