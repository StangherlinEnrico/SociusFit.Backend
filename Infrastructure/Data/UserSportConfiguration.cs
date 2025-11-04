using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework configuration for UserSport entity
/// </summary>
public class UserSportConfiguration : IEntityTypeConfiguration<UserSport>
{
    public void Configure(EntityTypeBuilder<UserSport> builder)
    {
        builder.ToTable("user_sports");

        builder.HasKey(us => us.Id);
        builder.Property(us => us.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(us => us.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(us => us.SportId)
            .HasColumnName("sport_id")
            .IsRequired();

        builder.Property(us => us.LevelId)
            .HasColumnName("level_id")
            .IsRequired();

        builder.Property(us => us.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(us => us.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(us => new { us.UserId, us.SportId })
            .IsUnique()
            .HasDatabaseName("IX_user_sports_user_sport");

        builder.HasIndex(us => us.UserId)
            .HasDatabaseName("IX_user_sports_user_id");

        builder.HasIndex(us => us.SportId)
            .HasDatabaseName("IX_user_sports_sport_id");
    }
}