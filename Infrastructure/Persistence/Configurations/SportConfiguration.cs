using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Configurazione Sport con seeding automatico da SportConstants
/// </summary>
public class SportConfiguration : IEntityTypeConfiguration<Sport>
{
    public void Configure(EntityTypeBuilder<Sport> builder)
    {
        builder.ToTable("Sports");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.Name)
            .IsUnique();

        // ✅ SEED DATA: Popola automaticamente sport predefiniti
        // Gli ID sono generati deterministicamente per evitare conflitti
        var sports = SportConstants.PredefinedSports
            .Select((name, index) => new
            {
                // Genera ID deterministico basato sul nome
                // Stesso nome = stesso ID in tutti gli ambienti
                Id = GenerateDeterministicGuid(name),
                Name = name,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            })
            .ToArray();

        builder.HasData(sports);
    }

    /// <summary>
    /// Genera un GUID deterministico basato su una stringa
    /// Garantisce che "Tennis" abbia sempre lo stesso ID
    /// </summary>
    private static Guid GenerateDeterministicGuid(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}