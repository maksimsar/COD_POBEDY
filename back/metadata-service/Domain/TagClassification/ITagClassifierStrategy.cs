namespace MetadataService.Domain.TagClassification;

public interface ITagClassifierStrategy
{
    Task<IReadOnlyList<string>> ClassifyAsync(string lyrics, CancellationToken ct = default);
}