using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Chat;

public class GetConversationsUseCase
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetConversationsUseCase> _logger;

    public GetConversationsUseCase(
        IMatchRepository matchRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IMapper mapper,
        ILogger<GetConversationsUseCase> logger)
    {
        _matchRepository = matchRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ConversationDto>>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var matches = await _matchRepository.GetUserMatchesAsync(userId, cancellationToken);
        var conversations = new List<ConversationDto>();

        foreach (var match in matches)
        {
            var otherUserId = match.GetOtherUserId(userId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId, cancellationToken);
            var otherProfile = await _profileRepository.GetByUserIdAsync(otherUserId, cancellationToken);

            if (otherUser == null)
            {
                _logger.LogWarning("User not found for match {MatchId}", match.Id);
                continue;
            }

            var messages = await _messageRepository.GetByMatchIdAsync(match.Id, 1, 1, cancellationToken);
            var lastMessage = messages.FirstOrDefault();
            var unreadCount = await _messageRepository.GetUnreadCountAsync(match.Id, userId, cancellationToken);

            conversations.Add(new ConversationDto
            {
                MatchId = match.Id,
                OtherUserId = otherUserId,
                OtherUserName = otherUser.FirstName,
                OtherUserPhotoUrl = otherProfile?.PhotoUrl,
                LastMessage = lastMessage != null ? _mapper.Map<MessageDto>(lastMessage) : null,
                UnreadCount = unreadCount
            });
        }

        var sortedConversations = conversations
            .OrderByDescending(c => c.LastMessage?.SentAt ?? DateTime.MinValue)
            .ToList();

        return Result<List<ConversationDto>>.Success(sortedConversations);
    }
}