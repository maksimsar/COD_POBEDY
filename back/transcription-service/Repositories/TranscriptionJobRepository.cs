using Microsoft.EntityFrameworkCore;
using TranscriptionService.Data;
using TranscriptionService.Messaging.Contracts;
using TranscriptionService.Models;

namespace TranscriptionService.Repositories;

public sealed class TranscriptionJobRepository : ITranscriptionJobRepository
{
    private readonly TranscriptionContext _context;
    public TranscriptionJobRepository(TranscriptionContext context) => _context = context;

    public Task<TranscriptionJob?>GetAsync(long id, CancellationToken ct = default) => 
        _context.TranscriptionJobs.AsNoTracking().FirstOrDefaultAsync(trj => trj.Id == id, ct);

    public async Task<IReadOnlyList<TranscriptionJob>> GetByStatusAsync(string status, CancellationToken ct = default)
    {
        return await _context.TranscriptionJobs
            .AsNoTracking()
            .Where(trj => trj.Status == status)
            .ToListAsync(ct);
    }
    
    public void Add(TranscriptionJob job) => _context.TranscriptionJobs.Add(job);
    public void Update(TranscriptionJob job) => _context.TranscriptionJobs.Update(job);
    
    public void AddOutbox(OutboxMessage message) => _context.OutboxMessages.Add(message);
    
    public Task SaveAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    
}