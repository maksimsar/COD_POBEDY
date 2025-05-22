using Common.DTOs;

namespace Common.Events;

public sealed record TranscriptReadyV1(
    Guid SongId,
    IReadOnlyList<TranscriptSegmentDto> Segments
);