using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProfileSportConfiguration : IEntityTypeConfiguration<ProfileSport>
{
    public void Configure(EntityTypeBuilder<ProfileSport> builder)
    {
        builder.ToTable("ProfileSports");

        // Chiave composta
        builder.HasKey(ps => new { ps.ProfileId, ps.SportId });

        builder.Property(ps => ps.Level)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        // Relazione con Profile
        builder.HasOne(ps => ps.Profile)
            .WithMany(p => p.ProfileSports)
            .HasForeignKey(ps => ps.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relazione con Sport
        builder.HasOne(ps => ps.Sport)
            .WithMany()
            .HasForeignKey(ps => ps.SportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ps => ps.ProfileId);
        builder.HasIndex(ps => ps.SportId);
    }
}