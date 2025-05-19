namespace UploadService.DTOs;

public class UploadRequest
{
    public IFormFile File { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string? Author { get; set; }
}