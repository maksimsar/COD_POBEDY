using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Application.Queries.Songs;
using MetadataService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetadataService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SongsController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SongDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetSongQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SongDto>>> List(
        [FromQuery] Guid? authorId,
        [FromQuery] int? tagId,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var list = await _mediator.Send(new ListSongsQuery(authorId, tagId, year), ct);
        return Ok(list);
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateSongRequest req,
        CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateSongCommand(req), ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSongRequest req,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateSongCommand(id, req), ct);
        return NoContent();
    }
    
    [HttpPut("{id:guid}/tags")]
    public async Task<IActionResult> UpdateTags(
        Guid id,
        [FromBody] UpdateTagsRequest req,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateSongTagsCommand(req), ct);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteSongCommand(id), ct);
        return NoContent();
    }
}
