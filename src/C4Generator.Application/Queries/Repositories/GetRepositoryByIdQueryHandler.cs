using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public sealed class GetRepositoryByIdQueryHandler : IRequestHandler<GetRepositoryByIdQuery, RepositoryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRepositoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RepositoryDto> Handle(GetRepositoryByIdQuery request, CancellationToken cancellationToken)
    {
        var repository = await _unitOfWork.Repositories.GetByIdAsync(request.RepositoryId, cancellationToken)
            ?? throw new NotFoundException($"Repository with ID '{request.RepositoryId}' was not found.");

        return new RepositoryDto(
            repository.Id, repository.Name, repository.Owner, repository.Url,
            repository.Description, repository.DefaultBranch, repository.Language,
            repository.ArchitectureStatus, repository.CreatedAt, repository.UpdatedAt
        );
    }
}
