using MetadataService.Configuration;
using Microsoft.Extensions.Options;

namespace MetadataService.Adapters;

public sealed class AudioTranscriberHttpAdapter : IAudioTranscriber
{
    private readonly HttpClient     _http;
    private readonly SwaggerSettings _cfg;

    public AudioTranscriberHttpAdapter(
        HttpClient            http,
        IOptions<SwaggerSettings> cfg)
    {
        _http = http;
        _cfg  = cfg.Value;
    }

    public async Task<IReadOnlyList<TranscriptSegment>> TranscribeAsync(
        Stream pcmAudio,
        string languageCode,
        CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();

        // 1) добавляем файл (PCM) в multipart
        content.Add(new StreamContent(pcmAudio), "file", "audio.pcm");
        content.Add(new StringContent(languageCode), "language");

        // 2) POST /transcribe
        var resp = await _http.PostAsync(_cfg.BaseUrl + "/api/transcribe", content, ct);
        resp.EnsureSuccessStatusCode();

        // 3) читаем JSON-ответ
        var segments = await resp.Content.ReadFromJsonAsync<List<TranscriptSegmentDto>>(cancellationToken: ct)
                       ?? new List<TranscriptSegmentDto>();

        // 4) конвертируем в доменный тип
        return segments.Select(s => new TranscriptSegment(
            s.Index,
            TimeSpan.FromMilliseconds(s.StartMs),
            TimeSpan.FromMilliseconds(s.EndMs),
            s.Text,
            s.Confidence
        )).ToList();
    }

    private record TranscriptSegmentDto(
        int    Index,
        long   StartMs,
        long   EndMs,
        string Text,
        float  Confidence
    );
}