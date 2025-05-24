namespace AuthService.Configuration;

public record JwtSettings
{
    public string Issuer      { get; init; } = default!;
    public string Audience    { get; init; } = default!;
    public string Secret      { get; init; } = default!;
    public int    AccessLife  { get; init; } = 15;   // минут
    public int    RefreshLife { get; init; } = 30;   // дней
}