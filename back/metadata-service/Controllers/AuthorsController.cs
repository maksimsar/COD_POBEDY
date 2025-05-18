using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Application.Queries.Author;
using MetadataService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetadataService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetAuthorQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuthorDto>>> List(CancellationToken ct)
    {
        var list = await _mediator.Send(new ListAuthorsQuery(), ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateAuthorRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateAuthorCommand(req), ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAuthorRequest req, CancellationToken ct)
    {
        await _mediator.Send(new UpdateAuthorCommand(id, req), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAuthorCommand(id), ct);
        return NoContent();
    }
}