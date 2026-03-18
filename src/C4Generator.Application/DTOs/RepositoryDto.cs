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
    DateTime CreatedAt,
    DateTime UpdatedAt
);
