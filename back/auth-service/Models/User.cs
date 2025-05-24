namespace AuthService.Models;

public class User
{
    public Guid   Id           { get; set; } = Guid.NewGuid();
    public string Email        { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FullName     { get; set; } = string.Empty;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public bool     IsActive   { get; set; } = true;

    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
}