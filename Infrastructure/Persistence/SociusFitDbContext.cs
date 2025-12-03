using Domain.Entities;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SociusFitDbContext : DbContext
{
    public SociusFitDbContext(DbContextOptions<SociusFitDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<Sport> Sports { get; set; } = null!;
    public DbSet<ProfileSport> ProfileSports { get; set; } = null!;
    public DbSet<UserCredentials> UserCredentials { get; set; } = null!;
    public DbSet<RevokedToken> RevokedTokens { get; set; }
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<DeviceToken> DeviceTokens { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SociusFitDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}