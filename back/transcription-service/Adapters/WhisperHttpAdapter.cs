using System.Net.Http.Json;
using Common.Events;
using Common.DTOs;

namespace TranscriptionService.Adapters;

public sealed class WhisperHttpAdapter : ISttEngine
{
    private readonly HttpClient                   _http;
    private readonly ILogger<WhisperHttpAdapter>  _log;
    
    public WhisperHttpAdapter(HttpClient http, ILogger<WhisperHttpAdapter> log)
    {
        _http = http;
        _log  = log;
    }
    
    public async Task<IReadOnlyCollection<TranscriptSegmentDto>> RecognizeAsync(
        Stream wav,
        string lang,
        CancellationToken ct = default)
    {
        /* 1️⃣ Собираем multipart-form */
        using var form = new MultipartFormDataContent
        {
            { new StreamContent(wav) { Headers = { { "Content-Type", "audio/wav" } } }, "file", "audio.wav" },
            { new StringContent(lang), "language" }
        };

        /* 2️⃣ Отправляем POST /stt */
        using var resp = await _http.PostAsync("/stt", form, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            _log.LogError("Whisper STT failed: {Code} {Body}", (int)resp.StatusCode, body);
            throw new HttpRequestException($"STT returned {(int)resp.StatusCode}");
        }

        /* 3️⃣ Десериализуем JSON в DTO */
        var segments = await resp.Content
                           .ReadFromJsonAsync<List<TranscriptSegmentDto>>(cancellationToken: ct)
                       ?? new List<TranscriptSegmentDto>();

        _log.LogDebug("Whisper returned {Count} segments", segments.Count);
        return segments;
    }
}