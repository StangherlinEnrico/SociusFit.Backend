using Domain.Enums;

namespace Domain.Entities;

public class Sport
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public SportLevel Level { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Sport() { }

    public Sport(string name, SportLevel level)
    {
        Id = Guid.NewGuid();
        Name = name;
        Level = level;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLevel(SportLevel level)
    {
        Level = level;
    }
}