using Common.DTOs;

namespace TranscriptionService.DTOs;

public record TranscriptionJobDto(
    long   Id,
    Guid   AudioFileId,
    string ModelUsed,
    string Language,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    string? ErrorMessage,
    IReadOnlyCollection<TranscriptSegmentDto>? Segments
);