using C4Generator.Domain.Enums;

namespace C4Generator.Application.DTOs;

public record ArchitectureDto(
    Guid Id,
    Guid RepositoryId,
    ArchitectureStatus Status,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
