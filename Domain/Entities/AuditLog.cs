namespace Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking system changes
/// </summary>
public class AuditLog
{
    public int Id { get; private set; }
    public int? UserId { get; private set; }
    public string Action { get; private set; }
    public string TableName { get; private set; }
    public int? RecordId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public User? User { get; private set; }

    // Private constructor for EF Core
    private AuditLog() { }

    public AuditLog(
        string action,
        string tableName,
        int? recordId = null,
        int? userId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be empty", nameof(tableName));

        Action = action;
        TableName = tableName;
        RecordId = recordId;
        UserId = userId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
    }
}