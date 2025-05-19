namespace UploadService.Models;

public class AudioRecord
{
    public Guid Id { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string Author { get; set; } = "Неизвестный";
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}