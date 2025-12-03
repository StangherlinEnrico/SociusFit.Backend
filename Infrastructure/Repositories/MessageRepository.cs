using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly SociusFitDbContext _context;

    public MessageRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    public async Task<List<Message>> GetByMatchIdAsync(
        Guid matchId,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.MatchId == matchId)
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> CreateAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<int> GetUnreadCountAsync(Guid matchId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.MatchId == matchId && m.SenderId != userId && !m.IsRead)
            .CountAsync(cancellationToken);
    }

    public async Task MarkMessagesAsReadAsync(Guid matchId, Guid userId, CancellationToken cancellationToken = default)
    {
        var messages = await _context.Messages
            .Where(m => m.MatchId == matchId && m.SenderId != userId && !m.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.MarkAsRead();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}