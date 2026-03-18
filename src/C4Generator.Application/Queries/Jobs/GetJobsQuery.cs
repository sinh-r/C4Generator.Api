using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Jobs;

public record GetJobsQuery : IRequest<IReadOnlyList<JobStatusDto>>;
