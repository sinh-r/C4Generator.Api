using C4Generator.Domain.Enums;

namespace C4Generator.Application.DTOs;

public record RepositoryDto(
    Guid Id,
    string Name,
    string Owner,
    string Url,
    string? Description,
    string? DefaultBranch,
    string? Language,
    ArchitectureStatus ArchitectureStatus,
    SourceControlProvider Provider,
    string ExternalId,
    DateTime LastSyncedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
