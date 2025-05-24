using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AuthContext _db;

    public UserRepository(AuthContext db) => _db = db;
    
    public void Add(User user) => _db.Users.Add(user);
    
    public async Task<User?> GetAsync(Guid id, CancellationToken ct = default) =>
        await _db.Users
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _db.Users
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct = default) =>
        await _db.UserRoles
            .AnyAsync(ur => ur.UserId == userId &&
                            ur.Role.Name == roleName, ct);

    public Task SaveAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}