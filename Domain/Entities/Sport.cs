namespace Domain.Entities;

public class Sport
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Sport() { }

    public Sport(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}