using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

public sealed class DeleteSongHandler : IRequestHandler<DeleteSongCommand>
{
    private readonly ISongRepository _songRepo;
    private readonly IUnitOfWork     _uow;

    public DeleteSongHandler(ISongRepository songRepo, IUnitOfWork uow)
        => (_songRepo, _uow) = (songRepo, uow);

    public async Task Handle(DeleteSongCommand cmd, CancellationToken ct)
    {
        var song = await _songRepo.GetByIdAsync(cmd.SongId, ct)
                   ?? throw new KeyNotFoundException("Song not found");
        _songRepo.Remove(song);
        await _uow.SaveChangesAsync(ct);
    }
}