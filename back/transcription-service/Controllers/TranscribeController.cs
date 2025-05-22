using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranscriptionService.DTOs;
using TranscriptionService.Services;   // ITranscriptionService

namespace TranscriptionService.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class TranscribeController : ControllerBase
{
    private readonly ITranscriptionService _service;

    public TranscribeController(ITranscriptionService service) => _service = service;

    /// <summary>Получить состояние джобы.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TranscriptionJobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var dto = await _service.GetAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Повторить обработку для упавшего задания.</summary>
    [HttpPost("reprocess")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reprocess([FromBody] ReprocessRequest req, CancellationToken ct)
    {
        try
        {
            await _service.RetryAsync(req.JobId, ct);
            return Accepted();               // 202
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);   // 400 + причина
        }
    }
}