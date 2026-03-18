using C4Generator.Application.Queries.Architecture;
using C4Generator.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4Generator.Api.Controllers;

[ApiController]
[Route("architecture/{id:guid}")]
[Authorize]
public sealed class VisualizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public VisualizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("context")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetContext(Guid id, CancellationToken cancellationToken)
        => GetDiagram(id, DiagramLevel.Context, cancellationToken);

    [HttpGet("containers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetContainers(Guid id, CancellationToken cancellationToken)
        => GetDiagram(id, DiagramLevel.Containers, cancellationToken);

    [HttpGet("components")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetComponents(Guid id, CancellationToken cancellationToken)
        => GetDiagram(id, DiagramLevel.Components, cancellationToken);

    [HttpGet("classes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetClasses(Guid id, CancellationToken cancellationToken)
        => GetDiagram(id, DiagramLevel.Classes, cancellationToken);

    private async Task<IActionResult> GetDiagram(Guid id, DiagramLevel level, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVisualizationQuery(id, level), cancellationToken);
        return Ok(result);
    }
}
