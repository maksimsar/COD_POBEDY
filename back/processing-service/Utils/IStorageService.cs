using System;
using System.IO;
using System.Threading.Tasks;

namespace ProcessingService.Utils
{
    public interface IStorageService
    {
        Task<Stream> DownloadAsync(string url);
        Task<string> SaveProcessedAsync(Guid recordId, Stream data);
    }
}
