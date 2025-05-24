namespace AuthService.Models;

public class RefreshToken
{
    public Guid     Token      { get; set; } = Guid.NewGuid();
    public Guid     UserId     { get; set; }
    public User     User       { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset IssuedAt  { get; set; }

    public string   UserAgent  { get; set; } = string.Empty;
    public string   IpAddress  { get; set; } = string.Empty;
}