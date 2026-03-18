using C4Generator.Domain.Enums;

namespace C4Generator.Domain.Entities;

public class Repository
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? DefaultBranch { get; private set; }
    public string? Language { get; private set; }
    public ArchitectureStatus ArchitectureStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Repository() { }

    public static Repository Create(string name, string owner, string url, string? description = null)
    {
        return new Repository
        {
            Id = Guid.NewGuid(),
            Name = name,
            Owner = owner,
            Url = url,
            Description = description,
            ArchitectureStatus = ArchitectureStatus.NotGenerated,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string? description, string? defaultBranch, string? language)
    {
        Description = description;
        DefaultBranch = defaultBranch;
        Language = language;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetArchitectureStatus(ArchitectureStatus status)
    {
        ArchitectureStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
