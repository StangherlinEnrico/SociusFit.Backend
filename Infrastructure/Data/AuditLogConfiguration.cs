using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework configuration for AuditLog entity
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.UserId)
            .HasColumnName("user_id");

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.TableName)
            .HasColumnName("table_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.RecordId)
            .HasColumnName("record_id");

        builder.Property(a => a.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("text");

        builder.Property(a => a.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("text");

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("IX_audit_logs_user_id");

        builder.HasIndex(a => a.TableName)
            .HasDatabaseName("IX_audit_logs_table_name");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_audit_logs_created_at");

        builder.HasIndex(a => new { a.TableName, a.RecordId })
            .HasDatabaseName("IX_audit_logs_table_record");
    }
}