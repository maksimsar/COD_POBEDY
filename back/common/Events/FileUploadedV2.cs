namespace Common.Events;

public sealed record FileUploadedV2(
    Guid SongId,
    string StorageKey,
    string MimeType, 
    long FileSize);