using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Main database context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<UserConsent> UserConsents { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Sport> Sports { get; set; } = null!;
    public DbSet<Level> Levels { get; set; } = null!;
    public DbSet<UserSport> UserSports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConsentConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new SportConfiguration());
        modelBuilder.ApplyConfiguration(new LevelConfiguration());
        modelBuilder.ApplyConfiguration(new UserSportConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps automatically
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Modified)
                {
                    // UpdatedAt is set via entity method, but we ensure it's always updated
                    entry.Property(nameof(User.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is UserConsent consent)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(UserConsent.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Sport sport)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(Sport.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Level level)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(Level.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is UserSport userSport)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(UserSport.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}