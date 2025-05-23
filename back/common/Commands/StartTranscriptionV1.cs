namespace Common.Commands;

public sealed record StartTranscriptionV1(
    Guid SongId,
    string StorageKey
);