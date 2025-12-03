using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LikerUserId)
            .IsRequired();

        builder.Property(l => l.LikedUserId)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasIndex(l => new { l.LikerUserId, l.LikedUserId })
            .IsUnique();

        builder.HasIndex(l => l.LikerUserId);
        builder.HasIndex(l => l.LikedUserId);
    }
}