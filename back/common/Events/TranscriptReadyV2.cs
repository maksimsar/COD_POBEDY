using Common.DTOs;

namespace Common.Events;

public record TranscriptReadyV2(
    Guid AudioFileId,
    IReadOnlyList<TranscriptSegmentDto> Segments,
    string? TranscriptStorageKey);