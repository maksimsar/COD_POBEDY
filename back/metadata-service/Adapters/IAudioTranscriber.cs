namespace MetadataService.Adapters;

public interface IAudioTranscriber
{
    /// <summary>Отправляет аудио (PCM 16‑kHz, 16‑bit LE) и получает расшифровку.</summary>
    Task<IReadOnlyList<TranscriptSegment>> TranscribeAsync(
        Stream pcmAudio,
        string languageCode,
        CancellationToken ct = default);
}

public sealed record TranscriptSegment(
    int     Index,
    TimeSpan Start,
    TimeSpan End,
    string   Text,
    float    Confidence);