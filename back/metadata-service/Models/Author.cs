namespace MetadataService.Models;

public sealed class Author : BaseEntity
{
    public string FullName { get; set; } = null!;
    public short? BirthYear { get; set; }
    public short? DeathYear { get; set; }
    public string? Notes { get; set; }
    public ICollection<SongAuthor> SongAuthors { get; set; } = new List<SongAuthor>();
}