using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository(DbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByProviderAsync(
        string provider,
        string providerId,
        CancellationToken cancellationToken = default)
    {
        var normalizedProvider = provider.ToLowerInvariant();
        return await _dbSet
            .FirstOrDefaultAsync(
                u => u.Provider == normalizedProvider && u.ProviderId == providerId,
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();
        return await _dbSet
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token, cancellationToken);
    }
}