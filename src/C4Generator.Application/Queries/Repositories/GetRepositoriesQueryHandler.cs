using C4Generator.Application.DTOs;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public sealed class GetRepositoriesQueryHandler : IRequestHandler<GetRepositoriesQuery, IReadOnlyList<RepositoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRepositoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<RepositoryDto>> Handle(GetRepositoriesQuery request, CancellationToken cancellationToken)
    {
        var repositories = await _unitOfWork.Repositories.GetAllAsync(cancellationToken);

        return repositories.Select(r => new RepositoryDto(
            r.Id, r.Name, r.Owner, r.Url, r.Description,
            r.DefaultBranch, r.Language, r.ArchitectureStatus,
            r.CreatedAt, r.UpdatedAt
        )).ToList();
    }
}
