using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Jobs;

public record GetJobByIdQuery(Guid JobId) : IRequest<JobStatusDto>;
