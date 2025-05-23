using System.Linq.Expressions;

namespace MetadataService.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}