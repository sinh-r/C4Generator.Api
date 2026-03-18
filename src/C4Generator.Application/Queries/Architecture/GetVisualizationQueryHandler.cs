using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Enums;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Architecture;

public sealed class GetVisualizationQueryHandler : IRequestHandler<GetVisualizationQuery, VisualizationDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVisualizationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VisualizationDto> Handle(GetVisualizationQuery request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.Architectures.GetByIdAsync(request.ArchitectureId, cancellationToken)
            ?? throw new NotFoundException($"Architecture model with ID '{request.ArchitectureId}' was not found.");

        if (model.Status != ArchitectureStatus.Generated || model.ModelJson is null)
            throw new InvalidOperationException($"Architecture model '{request.ArchitectureId}' has not been generated yet. Current status: {model.Status}.");

        // Extract the requested level from the stored model JSON.
        // The worker stores all levels inside the model; we return the relevant sub-section here.
        var levelJson = ExtractLevel(model.ModelJson, request.Level);

        return new VisualizationDto(request.ArchitectureId, request.Level, levelJson);
    }

    private static string ExtractLevel(string modelJson, DiagramLevel level)
    {
        // Full extraction is performed by the worker; here we return the full JSON
        // tagged with the requested level so the frontend can filter it.
        return modelJson;
    }
}
