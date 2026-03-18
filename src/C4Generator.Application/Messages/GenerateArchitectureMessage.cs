namespace C4Generator.Application.Messages;

public record GenerateArchitectureMessage(
    Guid RepositoryId,
    Guid ArchitectureModelId,
    Guid JobId,
    string RepositoryUrl,
    string Branch
);
