using C4Generator.Application.Commands.Repositories;
using C4Generator.Application.Queries.Repositories;
using C4Generator.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4Generator.Api.Controllers;

[ApiController]
[Route("repositories")]
[Authorize]
public sealed class RepositoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public RepositoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRepositoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRepositoryCommand(request.Name, request.Owner, request.Url, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] SourceControlProvider provider,
        [FromQuery] RepositoryScope scope,
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var token = Request.Headers["X-Source-Token"].FirstOrDefault() ?? string.Empty;
        var query = new GetRepositoriesQuery(provider, scope, name, token, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRepositoryByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteRepositoryCommand(id), cancellationToken);
        return NoContent();
    }
}

public record CreateRepositoryRequest(string Name, string Owner, string Url, string? Description);
