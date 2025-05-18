using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class ApproveTranscriptHandler : IRequestHandler<ApproveTranscriptCommand, Unit>
{
    private readonly ITranscriptRepository _transcriptRepo;
    private readonly IUnitOfWork           _unitOfWork;

    public ApproveTranscriptHandler(ITranscriptRepository trRepo, IUnitOfWork uow)
    {
        _transcriptRepo = trRepo;
        _unitOfWork    = uow;
    }

    public async Task<Unit> Handle(ApproveTranscriptCommand cmd, CancellationToken ct)
    {
        var segment = await _transcriptRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Transcript {cmd.Id} not found");

        segment.CheckedAt  = DateTimeOffset.UtcNow;
        segment.CheckedById = cmd.CheckedById;
        await _unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}