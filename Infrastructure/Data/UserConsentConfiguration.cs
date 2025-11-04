using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework configuration for UserConsent entity
/// </summary>
public class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.ToTable("user_consents");

        builder.HasKey(uc => uc.Id);
        builder.Property(uc => uc.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(uc => uc.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(uc => uc.ConsentType)
            .HasColumnName("consent_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(uc => uc.IsGranted)
            .HasColumnName("is_granted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(uc => uc.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(uc => uc.GrantedAt)
            .HasColumnName("granted_at");

        builder.Property(uc => uc.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(uc => uc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(uc => uc.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(uc => uc.UserId)
            .HasDatabaseName("IX_user_consents_user_id");

        builder.HasIndex(uc => new { uc.UserId, uc.ConsentType })
            .HasDatabaseName("IX_user_consents_user_consent_type");
    }
}