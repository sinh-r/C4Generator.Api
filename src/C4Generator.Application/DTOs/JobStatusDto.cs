using C4Generator.Domain.Enums;

namespace C4Generator.Application.DTOs;

public record JobStatusDto(
    Guid Id,
    Guid RepositoryId,
    string JobType,
    JobStatus Status,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt
);
