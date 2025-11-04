using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderId == providerId, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersWithinDistanceAsync(
        decimal latitude,
        decimal longitude,
        int maxDistanceKm,
        CancellationToken cancellationToken = default)
    {
        // This is a simplified version. For production, you should use spatial queries
        // or a stored procedure for better performance with the Haversine formula
        var users = await _dbSet
            .Where(u => u.Latitude != null && u.Longitude != null)
            .ToListAsync(cancellationToken);

        // Filter by distance using Haversine formula
        const double earthRadiusKm = 6371;

        var nearbyUsers = users.Where(u =>
        {
            if (!u.Latitude.HasValue || !u.Longitude.HasValue)
                return false;

            var dLat = DegreesToRadians((double)(u.Latitude.Value - latitude));
            var dLon = DegreesToRadians((double)(u.Longitude.Value - longitude));

            var lat1 = DegreesToRadians((double)latitude);
            var lat2 = DegreesToRadians((double)u.Latitude.Value);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadiusKm * c;

            return distance <= maxDistanceKm;
        });

        return nearbyUsers;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public async Task<IEnumerable<User>> GetUsersBySportAsync(int sportId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.UserSports.Any(us => us.SportId == sportId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersBySportAndLevelAsync(
        int sportId,
        int levelId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.UserSports.Any(us => us.SportId == sportId && us.LevelId == levelId))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdWithSportsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserSports)
                .ThenInclude(us => us.Sport)
            .Include(u => u.UserSports)
                .ThenInclude(us => us.Level)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithSessionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Sessions)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithConsentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Consents)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}