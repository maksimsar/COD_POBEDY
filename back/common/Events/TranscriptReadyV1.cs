using Common.DTOs;

namespace Common.Events;

public sealed record TranscriptSegmentDto(
    int    Index,
    long   StartMs,
    long   EndMs,
    string Text,
    float  Confidence
);