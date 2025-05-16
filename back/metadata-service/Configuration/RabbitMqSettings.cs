namespace MetadataService.Configuration;

public sealed class RabbitMqSettings
{
    public string Host           { get; init; } = default!;
    public string VirtualHost    { get; init; } = "/";
    public string Username       { get; init; } = "guest";
    public string Password       { get; init; } = "guest";
    public string ExchangePrefix { get; init; } = "";
}