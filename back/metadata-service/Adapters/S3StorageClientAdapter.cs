using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MetadataService.Configuration;


namespace MetadataService.Adapters;

public sealed class S3StorageClientAdapter : IStorageClient
{
    private readonly IAmazonS3      _s3;
    private readonly StorageSettings _opt;

    public S3StorageClientAdapter(IAmazonS3 s3, IOptions<StorageSettings> opt)
    { _s3 = s3; _opt = opt.Value; }

    public async Task<Uri> UploadAsync(string key, Stream data, string type, CancellationToken ct = default)
    {
        var req = new PutObjectRequest
        {
            BucketName   = _opt.BucketName,
            Key          = key,
            InputStream  = data,
            ContentType  = type,
            AutoCloseStream = false
        };
        await _s3.PutObjectAsync(req, ct);
        return new Uri($"s3://{_opt.BucketName}/{key}");
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
    {
        var res = await _s3.GetObjectAsync(_opt.BucketName, key, ct);
        return res.ResponseStream;
    }

    public Task DeleteAsync(string key, CancellationToken ct = default)
        => _s3.DeleteObjectAsync(_opt.BucketName, key, ct);
}