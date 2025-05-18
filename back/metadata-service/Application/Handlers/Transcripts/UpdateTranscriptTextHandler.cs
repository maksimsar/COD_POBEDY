using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class UpdateTranscriptTextHandler : IRequestHandler<UpdateTranscriptTextCommand, Unit>
{
    private readonly ITranscriptRepository _transcriptRepo;
    private readonly IUnitOfWork           _unitOfWork;
    private readonly IMapper               _mapper;

    public UpdateTranscriptTextHandler(ITranscriptRepository trRepo, IUnitOfWork uow, IMapper mapper)
    {
        _transcriptRepo = trRepo;
        _unitOfWork    = uow;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTranscriptTextCommand cmd, CancellationToken ct)
    {
        var segment = await _transcriptRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Transcript {cmd.Id} not found");

        _mapper.Map(cmd.Request, segment);
        segment.CheckedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}