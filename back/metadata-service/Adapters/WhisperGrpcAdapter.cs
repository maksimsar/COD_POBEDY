/*
using MetadataService.Configuration;
using Microsoft.Extensions.Options;

namespace MetadataService.Adapters;

public sealed class WhisperGrpcAdapter : IAudioTranscriber
{
    private readonly WhisperGrpc.WhisperGrpcClient _client; // сгенерённый gRPC‑stub
    private readonly ILogger<WhisperGrpcAdapter>  _log;
    private readonly GrpcSettings _cfg;

    public WhisperGrpcAdapter(
        WhisperGrpc.WhisperGrpcClient client,
        IOptions<GrpcSettings> cfg,
        ILogger<WhisperGrpcAdapter> log)
    {
        _client = client;
        _cfg = cfg.Value;
        _log = log;
    }

    public async Task<IReadOnlyList<TranscriptSegment>> TranscribeAsync(
        Stream pcmAudio,
        string languageCode,
        CancellationToken ct = default)
    {
        var deadline = DateTime.UtcNow.AddSeconds(_cfg.DeadlineSeconds);
        using var call = _client.Transcribe(deadline: deadline, cancellationToken: ct);

        // 1) шлем метаданные запроса
        await call.RequestStream.WriteAsync(new TranscribeRequest
        {
            Config = new TranscribeConfig { Language = languageCode }
        });

        // 2) шлем аудио чанками по 64 KB
        var buffer = new byte[64 * 1024];
        int read;
        while ((read = await pcmAudio.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
        {
            await call.RequestStream.WriteAsync(new TranscribeRequest
            {
                AudioChunk = Google.Protobuf.ByteString.CopyFrom(buffer, 0, read)
            });
        }
        await call.RequestStream.CompleteAsync();

        // 3) получаем ответ
        var response = await call.ResponseAsync;
        var segments = new List<TranscriptSegment>(response.Segments.Count);
        foreach (var s in response.Segments)
            segments.Add(new TranscriptSegment(s.Index, TimeSpan.FromMilliseconds(s.StartMs),
                TimeSpan.FromMilliseconds(s.EndMs), s.Text, s.Confidence));

        _log.LogInformation("Whisper returned {Count} segments", segments.Count);
        return segments;
    }
}*/