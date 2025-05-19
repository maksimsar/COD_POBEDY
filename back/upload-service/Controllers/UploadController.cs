using Microsoft.AspNetCore.Mvc;
using UploadService.DTOs;
using UploadService.Repositories;

namespace UploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly AudioRecordRepository _repository;
    private readonly ILogger<UploadController> _logger;

    public UploadController(AudioRecordRepository repository, ILogger<UploadController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadRequest request)
    {
        try
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Файл не предоставлен");

            if (string.IsNullOrWhiteSpace(request.SongName))
                return BadRequest("Название песни обязательно");

            var audioRecord = await _repository.AddAudioRecordAsync(request);
            return Ok(new { audioRecord.Id, audioRecord.SongName, audioRecord.Author });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла");
            return StatusCode(500, "Произошла ошибка при обработке вашего запроса");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _repository.GetAllAudioRecordsAsync();
        return Ok(records.Select(r => new { r.Id, r.SongName, r.Author, r.UploadDate }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var record = await _repository.GetAudioRecordByIdAsync(id);
        if (record == null)
            return NotFound();

        return File(record.FileData, record.ContentType, record.FileName);
    }
}