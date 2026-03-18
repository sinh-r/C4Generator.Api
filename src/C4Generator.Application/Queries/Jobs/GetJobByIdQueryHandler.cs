using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Jobs;

public sealed class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobStatusDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetJobByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<JobStatusDto> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"Job with ID '{request.JobId}' was not found.");

        return new JobStatusDto(
            job.Id, job.RepositoryId, job.JobType, job.Status,
            job.ErrorMessage, job.CreatedAt, job.StartedAt, job.CompletedAt
        );
    }
}
