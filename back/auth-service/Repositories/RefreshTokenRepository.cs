using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthContext _db;
    public RefreshTokenRepository(AuthContext db) => _db = db;

    public void Add(RefreshToken token) => _db.RefreshTokens.Add(token);

    public Task<RefreshToken?> GetAsync(Guid token, CancellationToken ct = default) =>
        _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task InvalidateAsync(Guid token, CancellationToken ct = default)
    {
        var entity = await GetAsync(token, ct);
        if (entity is null) return;

        _db.RefreshTokens.Remove(entity);
    }

    public Task SaveAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}