using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("RevokedTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .ValueGeneratedNever();

        builder.Property(rt => rt.TokenId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.RevokedAt)
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.Reason)
            .HasMaxLength(200);

        // Indexes for performance
        builder.HasIndex(rt => rt.TokenId)
            .IsUnique(); // Token ID is unique

        builder.HasIndex(rt => rt.UserId);

        builder.HasIndex(rt => rt.ExpiresAt); // For cleanup queries

        builder.HasIndex(rt => rt.RevokedAt);
    }
}