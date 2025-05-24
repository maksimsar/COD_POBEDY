namespace Common.Events;

public sealed record FileUploadedV2(
    Guid AudioFileId,
    string StorageKey,
    string MimeType, 
    long FileSize);