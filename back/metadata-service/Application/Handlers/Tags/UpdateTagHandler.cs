using AutoMapper;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Unit>
{
    private readonly ITagRepository _tagRepo;
    private readonly IUnitOfWork    _uow;
    private readonly IMapper        _mapper;

    public UpdateTagHandler(ITagRepository tagRepo, IUnitOfWork uow, IMapper mapper)
    {
        _tagRepo = tagRepo;
        _uow     = uow;
        _mapper  = mapper;
    }

    public async Task<Unit> Handle(UpdateTagCommand cmd, CancellationToken ct)
    {
        var tag = await _tagRepo.GetByIdAsync(cmd.Id, ct)
                  ?? throw new KeyNotFoundException($"Tag {cmd.Id} not found");

        if (cmd.Request.Name is not null && cmd.Request.Name != tag.Name)
        {
            if (await _tagRepo.GetByNameAsync(cmd.Request.Name, ct) is not null)
                throw new InvalidOperationException($"Tag '{cmd.Request.Name}' already exists");
        }

        _mapper.Map(cmd.Request, tag);
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}