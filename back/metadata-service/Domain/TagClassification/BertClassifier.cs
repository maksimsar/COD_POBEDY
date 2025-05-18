namespace MetadataService.Domain.TagClassification;

public sealed class BertClassifier : ITagClassifierStrategy
{
    private readonly HttpClient _http;
    private readonly double     _threshold;

    public BertClassifier(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _http      = httpFactory.CreateClient("BertClassifier");
        _threshold = cfg.GetValue<double>("TagClassifier:Threshold", 0.35);
    }

    private record Prediction(string Tag, double Score);

    public async Task<IReadOnlyList<string>> ClassifyAsync(string lyrics, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/classify", new { text = lyrics }, ct);
        response.EnsureSuccessStatusCode();
        var preds = await response.Content.ReadFromJsonAsync<List<Prediction>>(cancellationToken: ct);
        return preds?.Where(p => p.Score >= _threshold)
            .Select(p => p.Tag.ToLowerInvariant())
            .Distinct()
            .ToList() ?? new List<string>();
    }
}