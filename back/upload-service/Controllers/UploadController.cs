using Microsoft.AspNetCore.Mvc;
using UploadService.Repositories;
using UploadService.DTOs;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace UploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly AudioRecordRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UploadController> _logger;

    public UploadController(
        AudioRecordRepository repository,
        IHttpClientFactory httpClientFactory,
        ILogger<UploadController> logger)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
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

            var httpClient = _httpClientFactory.CreateClient("SearchService");
            var response = await httpClient.PostAsJsonAsync(
                "api/search/index",
                new
                {
                    Id = audioRecord.Id,
                    SongName = audioRecord.SongName,
                    Author = audioRecord.Author,
                    UploadDate = audioRecord.UploadDate
                });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Ошибка индексации: {response.StatusCode}");
            }

            return Ok(new
            {
                audioRecord.Id,
                audioRecord.SongName,
                audioRecord.Author
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла");
            return StatusCode(500, "Internal Server Error");
        }
    }
}