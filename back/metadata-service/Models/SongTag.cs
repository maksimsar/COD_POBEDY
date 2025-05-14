namespace MetadataService.Models;

public sealed class SongTag
{
    public Guid SongId { get; init; }
    public int  TagId  { get; init; }
    public Song? Song  { get; set; }
    public Tag?  Tag   { get; set; }
}