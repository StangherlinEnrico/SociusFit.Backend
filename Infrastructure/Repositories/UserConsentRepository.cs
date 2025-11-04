using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for UserConsent entity
/// </summary>
public class UserConsentRepository : Repository<UserConsent>, IUserConsentRepository
{
    public UserConsentRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(uc => uc.UserId == userId)
            .OrderByDescending(uc => uc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserConsent?> GetByUserAndTypeAsync(
        int userId,
        string consentType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConsentType == consentType, cancellationToken);
    }

    public async Task<IEnumerable<UserConsent>> GetActiveConsentsByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(uc => uc.UserId == userId && uc.IsGranted && uc.RevokedAt == null)
            .ToListAsync(cancellationToken);
    }
}