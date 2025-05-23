using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MetadataService.Data;

namespace MetadataService.Repositories;

internal abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly MetadataContext _context;
    protected readonly DbSet<T> _set;

    protected RepositoryBase(MetadataContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(object id, CancellationToken ct = default) =>
        _set.FindAsync(new[] { id }, ct).AsTask();

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        List<T> list;
        if (predicate == null)
        {
            list = await _set.AsNoTracking().ToListAsync(ct);
        }
        else
        {
            list = await _set
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(ct);
        }
        return list;
    }

    public void Add(T entity)    => _set.Add(entity);
    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
}