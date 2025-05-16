using MediatR;
using AutoMapper;
using Common.Events;
using MassTransit;
using MetadataService.Application.Commands;
using MetadataService.Repositories;
using MetadataService.Messaging.Contracts;

namespace MetadataService.Application.Handlers.Songs;

public sealed class UpdateSongHandler : IRequestHandler<UpdateSongCommand>
{
    private readonly ISongRepository  _songRepo;
    private readonly IUnitOfWork      _uow;
    private readonly IMapper          _mapper;
    private readonly IPublishEndpoint _bus;

    public UpdateSongHandler(ISongRepository songRepo, IUnitOfWork uow,
        IMapper mapper, IPublishEndpoint bus)
    {
        _songRepo = songRepo;
        _uow      = uow;
        _mapper   = mapper;
        _bus      = bus;
    }

    public async Task Handle(UpdateSongCommand cmd, CancellationToken ct)
    {
        var song = await _songRepo.GetByIdAsync(cmd.SongId, ct)
                   ?? throw new KeyNotFoundException("Song not found");
        _mapper.Map(cmd.Request, song);
        await _uow.SaveChangesAsync(ct);
        await _bus.Publish(new SongUpdatedV1(song.Id), ct);
    }
}