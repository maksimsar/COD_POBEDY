namespace MetadataService.DTOs;

public sealed record SongDto(
    Guid Id,
    string Title,
    short? Year,
    string? Description,
    int? DurationSec,
    IReadOnlyList<TagDto> Tags,
    IReadOnlyList<AuthorDto> Authors,
    IReadOnlyList<TranscriptDto> Transcripts);