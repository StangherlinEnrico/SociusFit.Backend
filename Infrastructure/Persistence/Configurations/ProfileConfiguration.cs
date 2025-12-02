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

        builder.Property(p => p.Latitude)
            .IsRequired()
            .HasColumnType("decimal(10,7)"); // Precisione sufficiente per coordinate GPS

        builder.Property(p => p.Longitude)
            .IsRequired()
            .HasColumnType("decimal(10,7)");

        builder.Property(p => p.Bio)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.MaxDistance)
            .IsRequired()
            .HasDefaultValue(25);

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

        // Indici spaziali per query geografiche efficienti
        builder.HasIndex(p => new { p.Latitude, p.Longitude });

        // La relazione con ProfileSport è gestita da ProfileSportConfiguration
        builder.HasMany(p => p.ProfileSports)
            .WithOne(ps => ps.Profile)
            .HasForeignKey(ps => ps.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}