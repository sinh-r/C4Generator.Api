using C4Generator.Domain.Enums;

namespace C4Generator.Domain.Entities;

public class ArchitectureModel
{
    public Guid Id { get; private set; }
    public Guid RepositoryId { get; private set; }
    public ArchitectureStatus Status { get; private set; }
    public string? ModelJson { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Repository Repository { get; private set; } = null!;

    private ArchitectureModel() { }

    public static ArchitectureModel Create(Guid repositoryId)
    {
        return new ArchitectureModel
        {
            Id = Guid.NewGuid(),
            RepositoryId = repositoryId,
            Status = ArchitectureStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessing()
    {
        Status = ArchitectureStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsGenerated(string modelJson)
    {
        ModelJson = modelJson;
        Status = ArchitectureStatus.Generated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Status = ArchitectureStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
