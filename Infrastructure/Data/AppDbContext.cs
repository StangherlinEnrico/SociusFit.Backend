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
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
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
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}