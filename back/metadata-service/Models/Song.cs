namespace MetadataService.Models;

public sealed class Song : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? Year { get; set; }
    public int? DurationSec { get; set; }

    public SongStatus Status;
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;

    public ICollection<SongAuthor> SongAuthors { get; set; } = new List<SongAuthor>();
    public ICollection<SongTag>    SongTags    { get; set; } = new List<SongTag>();
    public ICollection<Transcript> Transcripts { get; set; } = new List<Transcript>();
}

public enum SongStatus
{
    Uploaded,
    Processed,
    Transcribed
}