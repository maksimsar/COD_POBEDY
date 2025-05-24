namespace AuthService.Models;

public class UserRole
{
    public Guid  UserId { get; set; }
    public User  User   { get; set; } = default!;

    public short RoleId { get; set; }
    public Role  Role   { get; set; } = default!;
}