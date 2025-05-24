using AuthService.Models;

namespace AuthService.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    
    Task<Role>  EnsureRoleAsync(string name, CancellationToken ct = default);
    
    void Add(Role role);
    
    Task SaveAsync(CancellationToken ct = default);
}