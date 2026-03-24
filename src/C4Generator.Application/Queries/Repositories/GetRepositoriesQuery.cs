using C4Generator.Application.DTOs;
using C4Generator.Domain.Enums;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public record GetRepositoriesQuery(
    SourceControlProvider Provider,
    RepositoryScope Scope,
    string Name,
    string Token,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<RepositoryDto>>;
