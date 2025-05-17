using MetadataService.Data;
using MetadataService.Repositories;

namespace MetadataService.Infrastructure.UnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly MetadataContext _ctx;
    public EfUnitOfWork(MetadataContext ctx) => _ctx = ctx;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) 
        => _ctx.SaveChangesAsync(ct);

    public void Dispose() 
        => _ctx.Dispose();
}