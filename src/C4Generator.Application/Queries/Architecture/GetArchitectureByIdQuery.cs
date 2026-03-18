using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Architecture;

public record GetArchitectureByIdQuery(Guid ArchitectureId) : IRequest<ArchitectureDto>;
