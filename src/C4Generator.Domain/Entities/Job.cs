using C4Generator.Domain.Enums;

namespace C4Generator.Domain.Entities;

public class Job
{
    public Guid Id { get; private set; }
    public Guid RepositoryId { get; private set; }
    public JobStatus Status { get; private set; }
    public string JobType { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Repository Repository { get; private set; } = null!;

    private Job() { }

    public static Job Create(Guid repositoryId, string jobType)
    {
        return new Job
        {
            Id = Guid.NewGuid(),
            RepositoryId = repositoryId,
            JobType = jobType,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRunning()
    {
        Status = JobStatus.Running;
        StartedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = JobStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Status = JobStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = JobStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }
}
