using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for AuditLog entity
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(
        int userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AuditLog>> GetByTableNameAsync(
        string tableName,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AuditLog>> GetByRecordAsync(
        string tableName,
        int recordId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}