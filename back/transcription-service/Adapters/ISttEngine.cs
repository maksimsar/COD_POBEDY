using Common.DTOs;

namespace TranscriptionService.Adapters;

public interface ISttEngine
{
    Task<IReadOnlyCollection<TranscriptSegmentDto>> RecognizeAsync(
        Stream          wav,
        string          lang,
        CancellationToken ct = default);
}