using Domain.Interfaces.Repositories;

namespace Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}