using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class UpdateAuthorHandler : IRequestHandler<UpdateAuthorCommand, Unit>
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IUnitOfWork       _uow;
    private readonly IMapper           _mapper;

    public UpdateAuthorHandler(
        IAuthorRepository authorRepo,
        IUnitOfWork       uow,
        IMapper           mapper)
    {
        _authorRepo = authorRepo;
        _uow        = uow;
        _mapper     = mapper;
    }

    public async Task<Unit> Handle(UpdateAuthorCommand cmd, CancellationToken ct)
    {
        var author = await _authorRepo.GetByIdAsync(cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"Author {cmd.Id} not found");

        _mapper.Map(cmd.Request, author);
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}