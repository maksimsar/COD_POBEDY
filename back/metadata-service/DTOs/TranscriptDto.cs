namespace MetadataService.DTOs;

public sealed record TranscriptDto(
    long Id,
    int SegmentIndex,
    int StartMs,
    int EndMs,
    string Text,
    decimal? Confidence);