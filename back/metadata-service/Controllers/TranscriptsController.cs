using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Application.Queries.Transcripts;
using MetadataService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MetadataService.Controllers;

[ApiController]
[Route("api/songs/{songId:guid}/[controller]")]
public class TranscriptsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TranscriptsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TranscriptDto>>> ListBySong(
        Guid songId,
        CancellationToken ct)
    {
        var list = await _mediator.Send(new ListTranscriptsQuery(songId), ct);
        return Ok(list);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateText(
        Guid songId,
        long id,
        UpdateTranscriptRequest req,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateTranscriptTextCommand(id, req), ct);
        return NoContent();
    }

    [HttpPut("{id:long}/approve")]
    public async Task<IActionResult> Approve(
        Guid songId,
        long id,
        CancellationToken ct)
    {
        await _mediator.Send(new ApproveTranscriptCommand(id), ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        Guid songId,
        long id,
        CancellationToken ct)
    {
        await _mediator.Send(new DeleteTranscriptCommand(id), ct);
        return NoContent();
    }
}