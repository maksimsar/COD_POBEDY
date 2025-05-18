namespace MetadataService.Adapters;

public interface IStorageClient
{
    Task<Uri>    UploadAsync(string key, Stream data, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string key, CancellationToken ct = default);
    Task         DeleteAsync(string key, CancellationToken ct = default);
}