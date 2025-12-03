using Application.Requests;
using Application.UseCases.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly SendMessageUseCase _sendMessageUseCase;
    private readonly GetMessagesUseCase _getMessagesUseCase;
    private readonly GetConversationsUseCase _getConversationsUseCase;
    private readonly MarkMessagesAsReadUseCase _markMessagesAsReadUseCase;

    public ChatController(
        SendMessageUseCase sendMessageUseCase,
        GetMessagesUseCase getMessagesUseCase,
        GetConversationsUseCase getConversationsUseCase,
        MarkMessagesAsReadUseCase markMessagesAsReadUseCase)
    {
        _sendMessageUseCase = sendMessageUseCase;
        _getMessagesUseCase = getMessagesUseCase;
        _getConversationsUseCase = getConversationsUseCase;
        _markMessagesAsReadUseCase = markMessagesAsReadUseCase;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _getConversationsUseCase.ExecuteAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpGet("{matchId}/messages")]
    public async Task<IActionResult> GetMessages(
        Guid matchId,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _getMessagesUseCase.ExecuteAsync(matchId, userId, pageSize, pageNumber, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpPost("{matchId}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid matchId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _sendMessageUseCase.ExecuteAsync(matchId, userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpPut("{matchId}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid matchId,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _markMessagesAsReadUseCase.ExecuteAsync(matchId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(new { success = true });
    }
}