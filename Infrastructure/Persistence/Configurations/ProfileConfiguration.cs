using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.Age)
            .IsRequired();

        builder.Property(p => p.Gender)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Bio)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.HasIndex(p => p.City);

        builder.HasIndex(p => p.Age);

        builder.HasMany(p => p.Sports)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "ProfileSports",
                j => j.HasOne<Sport>().WithMany().HasForeignKey("SportId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Profile>().WithMany().HasForeignKey("ProfileId").OnDelete(DeleteBehavior.Cascade)
            );
    }
}