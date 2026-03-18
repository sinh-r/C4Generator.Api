using C4Generator.Domain.Enums;

namespace C4Generator.Application.DTOs;

public record VisualizationDto(
    Guid ArchitectureId,
    DiagramLevel Level,
    string DiagramJson
);
