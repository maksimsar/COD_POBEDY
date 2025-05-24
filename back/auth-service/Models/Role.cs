namespace AuthService.Models;

public class Role
{
    public short  Id   { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
}