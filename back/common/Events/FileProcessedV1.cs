namespace Common.Events;

public sealed record FileProcessedV1(
    Guid SongId,
    string StorageKey
);