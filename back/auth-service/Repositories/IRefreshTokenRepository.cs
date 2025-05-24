using AuthService.Models;

namespace AuthService.Repositories;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);
    Task<RefreshToken?> GetAsync(Guid token, CancellationToken ct = default);
    Task InvalidateAsync(Guid token, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}