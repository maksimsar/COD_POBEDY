using Microsoft.AspNetCore.Mvc;
using SearchService.Services;
using SearchService.DTOs;
using SearchService.Models;
namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ElasticSearchService _searchService;

    public SearchController(ElasticSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("text")]
    public async Task<IActionResult> SearchByText(
        [FromQuery] string query,
        [FromQuery] int fuzziness = 2)
    {
        var results = await _searchService.SearchAsync(query, fuzziness);
        return Ok(results);
    }

    [HttpPost("index")]
    public async Task<IActionResult> IndexAudio([FromBody] AudioRecord record)
    {
        await _searchService.IndexAudioAsync(record);
        return Ok();
    }
}