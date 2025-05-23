using System.Linq.Expressions;
using MetadataService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MetadataService.Repositories;

internal sealed class CachingSongRepository : ISongRepository
{
    private readonly ISongRepository _inner;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public CachingSongRepository(ISongRepository inner, IMemoryCache cache) =>
        (_inner, _cache) = (inner, cache);

    public Task<Song?> GetByIdAsync(object id, CancellationToken ct = default) =>
        _cache.GetOrCreateAsync($"song:{id}", e =>
        {
            e.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetByIdAsync(id, ct);
        });

    public Task<Song?> GetFullAsync(Guid id, CancellationToken ct = default) =>
        _cache.GetOrCreateAsync($"song-full:{id}", e =>
        {
            e.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.GetFullAsync(id, ct);
        });

    // остальные методы проксируем без кеша
    public Task<IReadOnlyList<Song>> ListAsync(Expression<Func<Song, bool>>? p, CancellationToken ct = default)
        => _inner.ListAsync(p, ct);
    public void Add(Song entity)    => _inner.Add(entity);
    public void Update(Song entity) => _inner.Update(entity);
    public void Remove(Song entity) => _inner.Remove(entity);
}