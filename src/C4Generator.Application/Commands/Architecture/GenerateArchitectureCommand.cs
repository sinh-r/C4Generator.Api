using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Commands.Architecture;

public record GenerateArchitectureCommand(
    Guid RepositoryId,
    string? Branch
) : IRequest<ArchitectureDto>;
