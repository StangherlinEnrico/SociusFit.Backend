using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication;

public interface IAuthenticationService
{
    Task<(User User, string Token)> RegisterAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<(User User, string Token)> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly SociusFitDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
        SociusFitDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<(User User, string Token)> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower(), cancellationToken);

        if (existingUser != null)
            throw new UserAlreadyExistsException(user.Email);

        await _context.Users.AddAsync(user, cancellationToken);

        var credentials = new UserCredentials
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PasswordHash = _passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserCredentials.AddAsync(credentials, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return (user, token);
    }

    public async Task<(User User, string Token)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);

        if (user == null)
            throw new UserNotFoundException(email);

        var credentials = await _context.UserCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == user.Id, cancellationToken);

        if (credentials == null)
            throw new InvalidUserDataException("User credentials not found");

        if (!_passwordHasher.VerifyPassword(password, credentials.PasswordHash))
            throw new InvalidUserDataException("Invalid password");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return (user, token);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);

        var credentials = await _context.UserCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == userId, cancellationToken);

        if (credentials == null)
            throw new InvalidUserDataException("User credentials not found");

        if (!_passwordHasher.VerifyPassword(currentPassword, credentials.PasswordHash))
            throw new InvalidUserDataException("Current password is incorrect");

        credentials.PasswordHash = _passwordHasher.HashPassword(newPassword);
        credentials.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}