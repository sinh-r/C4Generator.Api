using C4Generator.Application.Commands.Architecture;
using C4Generator.Application.Queries.Architecture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4Generator.Api.Controllers;

[ApiController]
[Route("architecture")]
[Authorize]
public sealed class ArchitectureController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArchitectureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Generate([FromBody] GenerateArchitectureRequest request, CancellationToken cancellationToken)
    {
        var command = new GenerateArchitectureCommand(request.RepositoryId, request.Branch);
        var result = await _mediator.Send(command, cancellationToken);
        return AcceptedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetArchitectureByIdQuery(id), cancellationToken);
        return Ok(result);
    }
}

public record GenerateArchitectureRequest(Guid RepositoryId, string? Branch);
