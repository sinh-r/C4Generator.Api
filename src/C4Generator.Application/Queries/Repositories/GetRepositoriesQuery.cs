using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public record GetRepositoriesQuery : IRequest<IReadOnlyList<RepositoryDto>>;
