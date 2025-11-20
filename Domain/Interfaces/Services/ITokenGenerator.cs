namespace Domain.Interfaces.Services;

/// <summary>
/// Service for generating and validating authentication tokens
/// </summary>
public interface ITokenGenerator
{
    string GenerateToken();
    string GenerateRefreshToken();
}
