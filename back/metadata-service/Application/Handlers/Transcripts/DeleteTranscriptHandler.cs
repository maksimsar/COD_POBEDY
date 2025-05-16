using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class DeleteTranscriptHandler : IRequestHandler<DeleteTranscriptCommand, Unit>
{
    private readonly ITranscriptRepository _transcriptRepo;
    private readonly IUnitOfWork           _unitOfWork;

    public DeleteTranscriptHandler(ITranscriptRepository trRepo, IUnitOfWork uow)
    {
        _transcriptRepo = trRepo;
        _unitOfWork    = uow;
    }

    public async Task<Unit> Handle(DeleteTranscriptCommand cmd, CancellationToken ct)
    {
        var seg = await _transcriptRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Transcript {cmd.Id} not found");

        _transcriptRepo.Remove(seg);
        await _unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}