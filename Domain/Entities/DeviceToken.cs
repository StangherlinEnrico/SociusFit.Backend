namespace Domain.Entities;

public class DeviceToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public string Platform { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private DeviceToken() { }

    public DeviceToken(Guid userId, string token, string platform)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        Platform = platform;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateToken(string token)
    {
        Token = token;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}