using System.IO;
using System.Threading.Tasks;

namespace ProcessingService.Utils
{
    public interface IMlServiceClient
    {
        Task<Stream> DenoiseAsync(Stream audioStream);
    }
}
