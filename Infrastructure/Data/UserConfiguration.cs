using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("users");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.EmailVerifiedAt)
            .HasColumnName("email_verified_at");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255);

        builder.Property(u => u.Provider)
            .HasColumnName("provider")
            .HasMaxLength(50);

        builder.Property(u => u.ProviderId)
            .HasColumnName("provider_id")
            .HasMaxLength(255);

        // Location settings
        builder.Property(u => u.Location)
            .HasColumnName("location")
            .HasMaxLength(100);

        builder.Property(u => u.MaxDistance)
            .HasColumnName("max_distance");

        // Email verification fields
        builder.Property(u => u.EmailVerificationToken)
            .HasColumnName("email_verification_token")
            .HasMaxLength(255);

        builder.Property(u => u.EmailVerificationTokenExpiresAt)
            .HasColumnName("email_verification_token_expires_at");

        // Password reset fields
        builder.Property(u => u.PasswordResetToken)
            .HasColumnName("password_reset_token")
            .HasMaxLength(255);

        builder.Property(u => u.PasswordResetTokenExpiresAt)
            .HasColumnName("password_reset_token_expires_at");

        // Timestamps
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_users_email");

        builder.HasIndex(u => u.EmailVerificationToken)
            .HasDatabaseName("IX_users_email_verification_token")
            .HasFilter("[email_verification_token] IS NOT NULL");

        builder.HasIndex(u => u.PasswordResetToken)
            .HasDatabaseName("IX_users_password_reset_token")
            .HasFilter("[password_reset_token] IS NOT NULL");

        builder.HasIndex(u => new { u.Provider, u.ProviderId })
            .HasDatabaseName("IX_users_provider")
            .HasFilter("[provider] IS NOT NULL");

        builder.HasIndex(u => u.DeletedAt)
            .HasDatabaseName("IX_users_deleted_at");

        // Index for location-based queries
        builder.HasIndex(u => u.Location)
            .HasDatabaseName("IX_users_location")
            .HasFilter("[location] IS NOT NULL");

        // Relationships
        builder.HasMany(u => u.AuditLogs)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Query filter for soft delete
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}