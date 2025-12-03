using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("DeviceTokens");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.UserId)
            .IsRequired();

        builder.Property(dt => dt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(dt => dt.Platform)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dt => dt.CreatedAt)
            .IsRequired();

        builder.Property(dt => dt.UpdatedAt)
            .IsRequired();

        builder.Property(dt => dt.IsActive)
            .IsRequired();

        builder.HasIndex(dt => dt.UserId);
        builder.HasIndex(dt => dt.Token).IsUnique();
        builder.HasIndex(dt => new { dt.UserId, dt.IsActive });
    }
}