namespace Common.Events;

public sealed record FileProcessedV1(
    Guid AudioFileId,
    string StorageKey
);