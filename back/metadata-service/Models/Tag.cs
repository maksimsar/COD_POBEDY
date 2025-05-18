namespace MetadataService.Models;

public sealed class Tag
{
    public int  Id       { get; init; }
    public string Name   { get; set; } = null!;
    public bool  Approved{ get; set; } = false;
    public ICollection<SongTag> SongTags { get; set; } = new List<SongTag>();
}