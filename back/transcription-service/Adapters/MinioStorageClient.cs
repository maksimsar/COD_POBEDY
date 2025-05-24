using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using TranscriptionService.Configuration;
using TranscriptionService.Interfaces;

namespace TranscriptionService.Adapters;

public sealed class MinioStorageClient : IStorageClient
{
    private readonly IMinioClient _minio;
    private readonly string      _bucket;
    
    public MinioStorageClient(IOptions<StorageSettings> opt)
    { 
        var storage = opt.Value;
        _bucket = storage.Bucket;

        _minio = new MinioClient()
            .WithEndpoint  (storage.Endpoint)
            .WithCredentials(storage.AccessKey, storage.SecretKey)
            .WithSSL       (storage.UseSSL)
            .Build();

        if (!string.IsNullOrWhiteSpace(storage.Region))
            _minio = _minio.WithRegion(storage.Region);

        _minio = _minio.Build();
    }
    
    public async Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
    {
        var ms = new MemoryStream();

        var getArgs = new GetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(key)
            .WithCallbackStream(stream => stream.CopyTo(ms));

        await _minio.GetObjectAsync(getArgs, ct);
        ms.Position = 0;
        return ms;
    }
    
    public async Task UploadAsync(string key, Stream content, string contentType,
        CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);

        var putArgs = new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(key)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(putArgs, ct);
    }
    
    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var delArgs = new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(key);

        await _minio.RemoveObjectAsync(delArgs, ct);
    }
    
    public Task<string> GeneratePresignedUrlAsync(string key,
        int expiresInSeconds = 3600,
        CancellationToken ct = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(key)
            .WithExpiry(expiresInSeconds);

        return _minio.PresignedGetObjectAsync(args);
    }
    
    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_bucket);
        var found = await _minio.BucketExistsAsync(existsArgs, ct);

        if (!found)
        {
            var makeArgs = new MakeBucketArgs().WithBucket(_bucket);
            await _minio.MakeBucketAsync(makeArgs, ct);
        }
    }
}