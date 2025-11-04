namespace Application.DTOs.Sports;

/// <summary>
/// DTO for sport response
/// </summary>
public class SportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a sport
/// </summary>
public class CreateSportDto
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for level response
/// </summary>
public class LevelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a level
/// </summary>
public class CreateLevelDto
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding sport to user
/// </summary>
public class AddUserSportDto
{
    public int SportId { get; set; }
    public int LevelId { get; set; }
}

/// <summary>
/// DTO for updating user sport level
/// </summary>
public class UpdateUserSportDto
{
    public int LevelId { get; set; }
}