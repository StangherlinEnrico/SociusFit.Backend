using Domain.Interfaces.Repositories;

namespace Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISessionRepository Sessions { get; }
    IUserConsentRepository UserConsents { get; }
    IAuditLogRepository AuditLogs { get; }
    ISportRepository Sports { get; }
    ILevelRepository Levels { get; }
    IUserSportRepository UserSports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}