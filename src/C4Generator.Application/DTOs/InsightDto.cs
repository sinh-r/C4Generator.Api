namespace C4Generator.Application.DTOs;

public record InsightDto(
    Guid Id,
    Guid ArchitectureModelId,
    string Category,
    string Title,
    string Description,
    string Severity,
    DateTime DiscoveredAt
);
