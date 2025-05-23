using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers.Tags;

internal sealed class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Unit>
{
    private readonly ITagRepository  _tagRepo;
    private readonly ISongRepository _songRepo;
    private readonly IUnitOfWork     _uow;

    public DeleteTagHandler(ITagRepository tagRepo, ISongRepository songRepo, IUnitOfWork uow)
    {
        _tagRepo  = tagRepo;
        _songRepo = songRepo;
        _uow      = uow;
    }

    public async Task<Unit> Handle(DeleteTagCommand cmd, CancellationToken ct)
    {
        var tag = await _tagRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Tag {cmd.Id} not found");

        var linked = await _songRepo.ListAsync(s => s.SongTags.Any(st => st.TagId == cmd.Id), ct);
        if (linked.Any())
            throw new InvalidOperationException("Cannot delete tag linked to songs");

        _tagRepo.Remove(tag);
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}