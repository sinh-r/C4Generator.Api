namespace C4Generator.Domain.Entities;

public class Insight
{
    public Guid Id { get; private set; }
    public Guid ArchitectureModelId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;
    public DateTime DiscoveredAt { get; private set; }

    public ArchitectureModel ArchitectureModel { get; private set; } = null!;

    private Insight() { }

    public static Insight Create(Guid architectureModelId, string category, string title, string description, string severity)
    {
        return new Insight
        {
            Id = Guid.NewGuid(),
            ArchitectureModelId = architectureModelId,
            Category = category,
            Title = title,
            Description = description,
            Severity = severity,
            DiscoveredAt = DateTime.UtcNow
        };
    }
}
