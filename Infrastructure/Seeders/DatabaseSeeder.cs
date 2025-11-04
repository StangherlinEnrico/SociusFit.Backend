using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seeders;

/// <summary>
/// Database seeder for initial data
/// </summary>
public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeeder>? _logger;

    public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder>? logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    /// <summary>
    /// Seeds all initial data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await SeedLevelsAsync();
            await SeedSportsAsync();

            _logger?.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Seeds skill levels
    /// </summary>
    private async Task SeedLevelsAsync()
    {
        if (await _context.Levels.AnyAsync())
        {
            _logger?.LogInformation("Levels already seeded, skipping...");
            return;
        }

        var levels = new[]
        {
            new Level("Beginner"),
            new Level("Intermediate"),
            new Level("Advanced"),
            new Level("Expert"),
            new Level("Professional")
        };

        await _context.Levels.AddRangeAsync(levels);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Seeded {Count} levels", levels.Length);
    }

    /// <summary>
    /// Seeds common sports
    /// </summary>
    private async Task SeedSportsAsync()
    {
        if (await _context.Sports.AnyAsync())
        {
            _logger?.LogInformation("Sports already seeded, skipping...");
            return;
        }

        var sports = new[]
        {
            // Popular sports
            new Sport("Football"),
            new Sport("Basketball"),
            new Sport("Tennis"),
            new Sport("Volleyball"),
            new Sport("Running"),
            new Sport("Cycling"),
            new Sport("Swimming"),
            new Sport("Gym/Fitness"),
            
            // Racket sports
            new Sport("Badminton"),
            new Sport("Table Tennis"),
            new Sport("Squash"),
            new Sport("Padel"),
            
            // Team sports
            new Sport("Rugby"),
            new Sport("Baseball"),
            new Sport("Softball"),
            new Sport("Hockey"),
            
            // Water sports
            new Sport("Surfing"),
            new Sport("Sailing"),
            new Sport("Kayaking"),
            new Sport("Diving"),
            
            // Winter sports
            new Sport("Skiing"),
            new Sport("Snowboarding"),
            new Sport("Ice Skating"),
            
            // Combat sports
            new Sport("Boxing"),
            new Sport("Martial Arts"),
            new Sport("Judo"),
            new Sport("Karate"),
            new Sport("Taekwondo"),
            
            // Other sports
            new Sport("Golf"),
            new Sport("Climbing"),
            new Sport("Hiking"),
            new Sport("Yoga"),
            new Sport("Dance"),
            new Sport("Skateboarding"),
            new Sport("Roller Skating")
        };

        await _context.Sports.AddRangeAsync(sports);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Seeded {Count} sports", sports.Length);
    }
}