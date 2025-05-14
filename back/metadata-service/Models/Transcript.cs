namespace MetadataService.Models;

public sealed class Transcript
{
    public long   Id           { get; init; }
    public Guid   SongId       { get; init; }
    public int    SegmentIndex { get; init; }
    public int    StartMs      { get; init; }
    public int    EndMs        { get; init; }
    public string Text         { get; set; } = null!;
    public decimal? Confidence { get; set; }
    public Guid?  CheckedById  { get; set; }
    public DateTimeOffset? CheckedAt { get; set; }
    public Song? Song { get; set; }
}