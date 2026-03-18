using C4Generator.Application.Abstractions;
using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Application.Messages;
using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Commands.Architecture;

public sealed class GenerateArchitectureCommandHandler : IRequestHandler<GenerateArchitectureCommand, ArchitectureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueuePublisher _queuePublisher;

    public GenerateArchitectureCommandHandler(IUnitOfWork unitOfWork, IQueuePublisher queuePublisher)
    {
        _unitOfWork = unitOfWork;
        _queuePublisher = queuePublisher;
    }

    public async Task<ArchitectureDto> Handle(GenerateArchitectureCommand request, CancellationToken cancellationToken)
    {
        var repository = await _unitOfWork.Repositories.GetByIdAsync(request.RepositoryId, cancellationToken)
            ?? throw new NotFoundException($"Repository with ID '{request.RepositoryId}' was not found.");

        var model = ArchitectureModel.Create(repository.Id);
        await _unitOfWork.Architectures.AddAsync(model, cancellationToken);

        var job = Job.Create(repository.Id, "ArchitectureGeneration");
        await _unitOfWork.Jobs.AddAsync(job, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var message = new GenerateArchitectureMessage(
            repository.Id,
            model.Id,
            job.Id,
            repository.Url,
            request.Branch ?? repository.DefaultBranch ?? "main"
        );

        await _queuePublisher.PublishAsync(message, cancellationToken);

        return new ArchitectureDto(
            model.Id,
            model.RepositoryId,
            model.Status,
            model.ErrorMessage,
            model.CreatedAt,
            model.UpdatedAt
        );
    }
}
