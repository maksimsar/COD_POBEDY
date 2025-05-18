using AutoMapper;
using MetadataService.DTOs;
using MetadataService.Models;
using MetadataService.Repositories;

using MetadataService.Repositories;

namespace MetadataService.Application.Services;

internal sealed class TranscriptService : ITranscriptService
{
    private readonly ITranscriptRepository _transcriptRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    TranscriptService(
        ITranscriptRepository transcriptRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _transcriptRepo = transcriptRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TranscriptDto>> ListBySongAsycn(Guid songId, CancellationToken ct = default)
    {
        var segments = await _transcriptRepo.ListBySongAsync(songId, ct);
        return _mapper.Map<IReadOnlyList<TranscriptDto>>(segments);
    }

    public async Task ApproveAsync(long transcriptId, CancellationToken ct = default)
    {
        var segments = await _transcriptRepo.GetByIdAsync(transcriptId, ct)
            ?? throw new KeyNotFoundException($"Transcript with id {transcriptId} not found");
        if(segments.CheckedAt is not null) return;
        segments.CheckedAt = DateTimeOffset.UtcNow;
        segments.CheckedById = null;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateTextAsync(long transcriptId, UpdateTranscriptRequest request,
        CancellationToken ct = default)
    {
        var segments = await _transcriptRepo.GetByIdAsync(transcriptId, ct)
            ?? throw new KeyNotFoundException($"Transcript with id {transcriptId} not found");
        segments.Text = request.Text;
        segments.CheckedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long transcriptId, CancellationToken ct = default)
    {
        var segments = await _transcriptRepo.GetByIdAsync(transcriptId, ct)
            ?? throw new KeyNotFoundException($"Transcript with id {transcriptId} not found");
        _transcriptRepo.Remove(segments);
        await _unitOfWork.SaveChangesAsync(ct);
    }
    
}