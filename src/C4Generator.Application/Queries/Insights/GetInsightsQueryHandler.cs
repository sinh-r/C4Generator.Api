using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Insights;

public sealed class GetInsightsQueryHandler : IRequestHandler<GetInsightsQuery, IReadOnlyList<InsightDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInsightsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<InsightDto>> Handle(GetInsightsQuery request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.Architectures.GetByIdAsync(request.ArchitectureId, cancellationToken)
            ?? throw new NotFoundException($"Architecture model with ID '{request.ArchitectureId}' was not found.");

        var insights = await _unitOfWork.Insights.GetByArchitectureModelIdAsync(model.Id, cancellationToken);

        return insights.Select(i => new InsightDto(
            i.Id, i.ArchitectureModelId, i.Category,
            i.Title, i.Description, i.Severity, i.DiscoveredAt
        )).ToList();
    }
}
