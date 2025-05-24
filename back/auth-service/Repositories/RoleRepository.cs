using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AuthContext _db;
    private readonly DbSet<Role> _roles;

    public RoleRepository(AuthContext db)
    {
        _db    = db;
        _roles = db.Roles;
    }
    
    
    public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        _roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name, ct);
    
    public async Task<Role> EnsureRoleAsync(string name, CancellationToken ct = default)
    {
        var role = await _roles.FirstOrDefaultAsync(r => r.Name == name, ct);
        if (role is not null)
            return role;

        role = new Role { Name = name.Trim() };
        _roles.Add(role);
        await _db.SaveChangesAsync(ct); 

        return role;
    }

    public void Add(Role role) => _roles.Add(role);

    public Task SaveAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}