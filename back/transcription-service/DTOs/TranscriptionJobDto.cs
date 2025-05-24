using Common.DTOs;

namespace TranscriptionService.DTOs;

public record TranscriptionJobDto(
        long             Id,
        Guid             AudioFileId,
        string           Status,
        DateTimeOffset?  StartedAt,
        DateTimeOffset?  FinishedAt,
        string?          ErrorMessage,
        string           Language,
        string           ModelUsed,
        DateTimeOffset   CreatedAt);