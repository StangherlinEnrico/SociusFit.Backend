namespace Application.DTOs.Users;

/// <summary>
/// DTO for user response
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string? Provider { get; set; }
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? MaxDistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for user with sports
/// </summary>
public class UserWithSportsDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? MaxDistanceKm { get; set; }
    public List<UserSportDto> Sports { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for user sport
/// </summary>
public class UserSportDto
{
    public int Id { get; set; }
    public string SportName { get; set; } = string.Empty;
    public int SportId { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int LevelId { get; set; }
}

/// <summary>
/// DTO for creating a user
/// </summary>
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating user profile
/// </summary>
public class UpdateUserProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Location { get; set; }
}

/// <summary>
/// DTO for updating user location
/// </summary>
public class UpdateUserLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int MaxDistanceKm { get; set; }
}

/// <summary>
/// DTO for user authentication
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for OAuth authentication
/// </summary>
public class OAuthLoginDto
{
    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO for user search with location
/// </summary>
public class UserSearchDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Location { get; set; }
    public double? DistanceKm { get; set; }
    public List<UserSportDto> Sports { get; set; } = new();
}