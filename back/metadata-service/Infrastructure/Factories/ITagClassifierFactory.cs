using MetadataService.Domain.TagClassification;

namespace MetadataService.Infrastructure.Factories;

public interface ITagClassifierFactory
{
    ITagClassifierStrategy Create();
}