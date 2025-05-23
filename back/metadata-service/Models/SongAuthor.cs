namespace MetadataService.Models;

public sealed class SongAuthor
{
    public Guid SongId   { get; init; }
    public Guid AuthorId { get; init; }
    public string Role   { get; init; } = "composer";

    public Song?  Song  { get; set; }
    public Author? Author { get; set; }
}