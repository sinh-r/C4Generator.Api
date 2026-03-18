using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Commands.Repositories;

public sealed class DeleteRepositoryCommandHandler : IRequestHandler<DeleteRepositoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRepositoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRepositoryCommand request, CancellationToken cancellationToken)
    {
        var repository = await _unitOfWork.Repositories.GetByIdAsync(request.RepositoryId, cancellationToken)
            ?? throw new NotFoundException($"Repository with ID '{request.RepositoryId}' was not found.");

        _unitOfWork.Repositories.Delete(repository);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
