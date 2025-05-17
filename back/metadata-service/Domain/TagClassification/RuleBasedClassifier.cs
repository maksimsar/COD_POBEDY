using System.Text.RegularExpressions;

namespace MetadataService.Domain.TagClassification;

public sealed class RuleBasedClassifier : ITagClassifierStrategy
{
    private readonly IDictionary<string, string[]> _rules; // pattern → tag names

    public RuleBasedClassifier(IDictionary<string, string[]> rules)
    {
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));
    }

    public Task<IReadOnlyList<string>> ClassifyAsync(string lyrics, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(lyrics)) return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        var text = lyrics.ToLowerInvariant();
        var found = new HashSet<string>();
        foreach (var kvp in _rules)
        {
            if (Regex.IsMatch(text, kvp.Key, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                foreach (var tag in kvp.Value)
                    found.Add(tag.ToLowerInvariant());
        }
        return Task.FromResult((IReadOnlyList<string>)found.ToList());
    }
}