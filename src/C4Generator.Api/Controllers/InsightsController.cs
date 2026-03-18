using C4Generator.Application.Queries.Insights;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4Generator.Api.Controllers;

[ApiController]
[Route("architecture/{id:guid}/insights")]
[Authorize]
public sealed class InsightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InsightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInsights(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInsightsQuery(id), cancellationToken);
        return Ok(result);
    }
}
