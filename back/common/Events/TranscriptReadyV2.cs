using Common.DTOs;

namespace Common.Events;

public record TranscriptReadyV2(
    Guid SongId,
    IReadOnlyList<TranscriptSegmentDto> Segments,
    string? TranscriptStorageKey);