namespace C4Generator.Application.Messages;

public record GenerateArchitectureMessage(
    Guid RepositoryId,
    Guid ArchitectureModelId,
    string RepositoryUrl,
    string Branch
);
