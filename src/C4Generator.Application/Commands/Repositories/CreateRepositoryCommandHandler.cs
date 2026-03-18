using C4Generator.Application.DTOs;
using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Commands.Repositories;

public sealed class CreateRepositoryCommandHandler : IRequestHandler<CreateRepositoryCommand, RepositoryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRepositoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RepositoryDto> Handle(CreateRepositoryCommand request, CancellationToken cancellationToken)
    {
        var repository = Repository.Create(request.Name, request.Owner, request.Url, request.Description);

        await _unitOfWork.Repositories.AddAsync(repository, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RepositoryDto(
            repository.Id,
            repository.Name,
            repository.Owner,
            repository.Url,
            repository.Description,
            repository.DefaultBranch,
            repository.Language,
            repository.ArchitectureStatus,
            repository.CreatedAt,
            repository.UpdatedAt
        );
    }
}
