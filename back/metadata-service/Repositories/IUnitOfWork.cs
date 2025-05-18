namespace MetadataService.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}