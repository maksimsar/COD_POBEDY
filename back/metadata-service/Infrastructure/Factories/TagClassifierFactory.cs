using MetadataService.Domain.TagClassification;

namespace MetadataService.Infrastructure.Factories;

public sealed class TagClassifierFactory : ITagClassifierFactory
{
    private readonly IServiceProvider _sp;
    private readonly IConfiguration _cfg;

    public TagClassifierFactory(IServiceProvider sp, IConfiguration cfg)
    {
        _sp = sp;
        _cfg = cfg;
    }

    public ITagClassifierStrategy Create() => (_cfg["TagClassifier:Type"] ?? "rule") switch
    {
        "rule" => _sp.GetRequiredService<RuleBasedClassifier>(),
        "bert" => _sp.GetRequiredService<BertClassifier>(),
        _ => throw new InvalidOperationException("Unknown TagClassifier type")
    };
}