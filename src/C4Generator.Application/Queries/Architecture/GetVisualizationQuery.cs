using C4Generator.Application.DTOs;
using C4Generator.Domain.Enums;
using MediatR;

namespace C4Generator.Application.Queries.Architecture;

public record GetVisualizationQuery(Guid ArchitectureId, DiagramLevel Level) : IRequest<VisualizationDto>;
