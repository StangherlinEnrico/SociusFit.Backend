using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for UserConsent entity
/// </summary>
public interface IUserConsentRepository : IRepository<UserConsent>
{
    Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<UserConsent?> GetByUserAndTypeAsync(
        int userId,
        string consentType,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<UserConsent>> GetActiveConsentsByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);
}