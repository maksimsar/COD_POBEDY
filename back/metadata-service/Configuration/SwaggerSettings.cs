namespace MetadataService.Configuration;

public sealed class SwaggerSettings
{
    public string BaseUrl { get; init; } = default!;
    public int    TimeoutSeconds { get; init; } = 60;
}