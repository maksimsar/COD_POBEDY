namespace Common.Events;

public record SongUpdatedV2(
    Guid   SongId,
    string OriginalStorageKey,
    string ProcessedStorageKey,
    string Title,
    int?   Year
);