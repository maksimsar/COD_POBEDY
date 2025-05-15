namespace MetadataService.DTOs;

public sealed record CreateSongRequest(
    string Title,
    short? Year,
    string? Description,
    int? DurationSec,
    IReadOnlyList<Guid> AuthorIds,
    IReadOnlyList<int> TagIds);