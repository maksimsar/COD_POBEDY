namespace TranscriptionService.Configuration;

public record StorageSettings
{
    public string Endpoint   { get; init; } = default!;
    
    public string Bucket     { get; init; } = "audio";

    public string AccessKey  { get; init; } = default!;
    public string SecretKey  { get; init; } = default!;
    
    public bool   UseSSL     { get; init; } = false;

    public string? Region { get; init; } = null;
}