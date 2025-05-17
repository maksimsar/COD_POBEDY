using MetadataService.Data;

namespace MetadataService.Repositories;

internal abstract class UnitOfWorkRepositoryBase<T> : RepositoryBase<T>, IUnitOfWork where T : class
{
    protected UnitOfWorkRepositoryBase(MetadataContext context) : base(context) { }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}