using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserCredentialsConfiguration : IEntityTypeConfiguration<UserCredentials>
{
    public void Configure(EntityTypeBuilder<UserCredentials> builder)
    {
        builder.ToTable("UserCredentials");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.Id)
            .ValueGeneratedNever();

        builder.Property(uc => uc.UserId)
            .IsRequired();

        builder.Property(uc => uc.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(uc => uc.CreatedAt)
            .IsRequired();

        builder.Property(uc => uc.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(uc => uc.UserId)
            .IsUnique();
    }
}