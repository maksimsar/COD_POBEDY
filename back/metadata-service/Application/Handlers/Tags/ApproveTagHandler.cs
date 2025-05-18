using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers.Tags;

internal sealed class ApproveTagHandler : IRequestHandler<ApproveTagCommand, Unit>
{
    private readonly ITagRepository _tagRepo;
    private readonly IUnitOfWork    _uow;

    public ApproveTagHandler(ITagRepository tagRepo, IUnitOfWork uow)
    {
        _tagRepo = tagRepo;
        _uow     = uow;
    }

    public async Task<Unit> Handle(ApproveTagCommand cmd, CancellationToken ct)
    {
        var tag = await _tagRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Tag {cmd.Id} not found");

        if (!tag.Approved)
        {
            tag.Approved = true;
            await _uow.SaveChangesAsync(ct);
        }
        return Unit.Value;
    }
}