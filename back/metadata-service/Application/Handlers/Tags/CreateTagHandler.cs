using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class CreateTagHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly ITagRepository _tagRepo;
    private readonly IUnitOfWork    _uow;
    private readonly IMapper        _mapper;

    public CreateTagHandler(ITagRepository tagRepo, IUnitOfWork uow, IMapper mapper)
    {
        _tagRepo = tagRepo;
        _uow     = uow;
        _mapper  = mapper;
    }

    public async Task<int> Handle(CreateTagCommand cmd, CancellationToken ct)
    {
        if (await _tagRepo.GetByNameAsync(cmd.Request.Name, ct) is not null)
            throw new InvalidOperationException($"Tag '{cmd.Request.Name}' already exists");

        var tag = _mapper.Map<Tag>(cmd.Request);
        _tagRepo.Add(tag);
        await _uow.SaveChangesAsync(ct);
        return tag.Id;
    }
}