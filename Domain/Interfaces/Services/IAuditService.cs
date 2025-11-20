namespace Domain.Interfaces.Services;

/// <summary>
/// Service for audit logging
/// </summary>
public interface IAuditService
{
    Task LogAsync(
        string action,
        string tableName,
        int? recordId = null,
        int? userId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}
