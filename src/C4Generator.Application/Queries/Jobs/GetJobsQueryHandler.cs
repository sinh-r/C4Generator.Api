using C4Generator.Application.DTOs;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Jobs;

public sealed class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, IReadOnlyList<JobStatusDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetJobsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<JobStatusDto>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _unitOfWork.Jobs.GetAllAsync(cancellationToken);

        return jobs.Select(j => new JobStatusDto(
            j.Id, j.RepositoryId, j.JobType, j.Status,
            j.ErrorMessage, j.CreatedAt, j.StartedAt, j.CompletedAt
        )).ToList();
    }
}
