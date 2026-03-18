using C4Generator.Application.Queries.Jobs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4Generator.Api.Controllers;

[ApiController]
[Route("jobs")]
[Authorize]
public sealed class JobController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetJobsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{jobId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid jobId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(jobId), cancellationToken);
        return Ok(result);
    }
}
