namespace TranscriptionService.Interfaces;

public interface IStorageClient
{
    Task<Stream> DownloadAsync(string key, CancellationToken ct = default);
    
    Task UploadAsync(string key, 
        Stream content,
        string contentType,
        CancellationToken ct = default);
    
    Task DeleteAsync(string key, CancellationToken ct = default);
    
    Task<string> GeneratePresignedUrlAsync(string key,
        int expiresInSeconds = 3600,
        CancellationToken ct = default);
}