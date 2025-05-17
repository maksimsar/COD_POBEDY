namespace Common.Events;

public sealed record TranscriptSegmentDto(
    int    Index,
    long   StartMs,
    long   EndMs,
    string Text,
    float  Confidence
);

public sealed record TranscriptReadyV1(
    Guid SongId,
    IReadOnlyList<TranscriptSegmentDto> Segments
);