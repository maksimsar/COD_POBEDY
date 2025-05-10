using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProcessingService.Utils
{
    public class FileStorageService : IStorageService
    {
        readonly string _up, _proc;
        public FileStorageService(IOptions<StorageSettings> opt)
        {
            _up = opt.Value.UploadsPath;
            _proc = opt.Value.ProcessedPath;
            Directory.CreateDirectory(_up);
            Directory.CreateDirectory(_proc);
        }

        public Task<Stream> DownloadAsync(string url)
        {
            var path = Path.Combine(_up, Path.GetFileName(url));
            return Task.FromResult<Stream>(File.OpenRead(path));
        }

        public async Task<string> SaveProcessedAsync(Guid id, Stream data)
        {
            var name = $"{id}.wav";
            var path = Path.Combine(_proc, name);
            data.Position = 0;
            await using var fs = File.Create(path);
            await data.CopyToAsync(fs);
            return name;
        }
    }
}
