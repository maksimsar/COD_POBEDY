namespace SearchService.Models;

public class AudioRecord
{
    public Guid Id { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}