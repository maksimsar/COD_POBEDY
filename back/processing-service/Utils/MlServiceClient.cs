using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProcessingService.Utils
{
    public class MlServiceClient : IMlServiceClient
    {
        readonly HttpClient _http;
        public MlServiceClient(HttpClient http) => _http = http;

        public async Task<Stream> DenoiseAsync(Stream audioStream)
        {
            using var content = new MultipartFormDataContent();
            audioStream.Position = 0;
            content.Add(new StreamContent(audioStream)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("audio/wav") }
            }, "file", "audio.wav");

            var resp = await _http.PostAsync("/denoise", content);
            resp.EnsureSuccessStatusCode();

            var ms = new MemoryStream();
            await (await resp.Content.ReadAsStreamAsync()).CopyToAsync(ms);
            ms.Position = 0;
            return ms;
        }
    }
}
