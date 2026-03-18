using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Architecture;

public sealed class GetArchitectureByIdQueryHandler : IRequestHandler<GetArchitectureByIdQuery, ArchitectureDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetArchitectureByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ArchitectureDto> Handle(GetArchitectureByIdQuery request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.Architectures.GetByIdAsync(request.ArchitectureId, cancellationToken)
            ?? throw new NotFoundException($"Architecture model with ID '{request.ArchitectureId}' was not found.");

        return new ArchitectureDto(
            model.Id, model.RepositoryId, model.Status,
            model.ErrorMessage, model.CreatedAt, model.UpdatedAt
        );
    }
}
