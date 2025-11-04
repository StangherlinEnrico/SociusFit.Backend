using System.Security.Cryptography;
using Domain.Interfaces.Services;

namespace Infrastructure.Services;

/// <summary>
/// Token generator service for authentication tokens
/// </summary>
public class TokenGenerator : ITokenGenerator
{
    private const int TokenLength = 64; // Length in bytes (will be 128 chars when converted to hex)

    public string GenerateToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[TokenLength];
        rng.GetBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public string GenerateRefreshToken()
    {
        // Same implementation, could be different if needed
        return GenerateToken();
    }
}