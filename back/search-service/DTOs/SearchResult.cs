namespace SearchService.DTOs;

public class SearchResult
{
    public Guid Id { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public double? Score { get; set; }
    public Dictionary<string, List<string>>? Highlights { get; set; }
}