using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Application.Queries.Tags;
using MetadataService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetadataService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TagsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDto>> Get(int id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetTagQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> List(CancellationToken ct)
    {
        var list = await _mediator.Send(new ListTagsQuery(), ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTagRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateTagCommand(req), ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTagRequest req, CancellationToken ct)
    {
        await _mediator.Send(new UpdateTagCommand(id, req), ct);
        return NoContent();
    }

    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        await _mediator.Send(new ApproveTagCommand(id), ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTagCommand(id), ct);
        return NoContent();
    }
}