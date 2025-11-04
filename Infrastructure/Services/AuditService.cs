using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;

namespace Infrastructure.Services;

/// <summary>
/// Audit service for logging system changes
/// </summary>
public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task LogAsync(
        string action,
        string tableName,
        int? recordId = null,
        int? userId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            action: action,
            tableName: tableName,
            recordId: recordId,
            userId: userId,
            oldValues: oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            newValues: newValues != null ? JsonSerializer.Serialize(newValues) : null,
            ipAddress: ipAddress
        );

        await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}