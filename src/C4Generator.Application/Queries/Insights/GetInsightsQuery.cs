using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Insights;

public record GetInsightsQuery(Guid ArchitectureId) : IRequest<IReadOnlyList<InsightDto>>;
