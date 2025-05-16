namespace MetadataService.Configuration;

public sealed class GrpcSettings
{
    public string Host            { get; init; } = default!; // host:port
    public int    DeadlineSeconds { get; init; } = 60;
}