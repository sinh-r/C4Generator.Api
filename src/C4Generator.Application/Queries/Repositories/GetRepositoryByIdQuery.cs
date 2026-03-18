using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public record GetRepositoryByIdQuery(Guid RepositoryId) : IRequest<RepositoryDto>;
