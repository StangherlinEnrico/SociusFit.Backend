using Domain.Entities;

namespace Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}