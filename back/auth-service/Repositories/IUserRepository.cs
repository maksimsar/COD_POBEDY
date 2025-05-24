using AuthService.Models;

namespace AuthService.Repositories;

public interface IUserRepository
{
    void Add(User user);
    
    Task<User?> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct = default);
    
    Task SaveAsync(CancellationToken ct = default);
    
    void AddOutbox(OutboxMessage message);
}