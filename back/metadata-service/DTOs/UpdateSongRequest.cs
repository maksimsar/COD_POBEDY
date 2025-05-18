namespace MetadataService.DTOs;

public sealed record UpdateSongRequest(
    string? Title,
    short? Year,
    string? Description,
    int? DurationSec);