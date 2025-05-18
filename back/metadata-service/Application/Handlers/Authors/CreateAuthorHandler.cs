using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class CreateAuthorHandler : IRequestHandler<CreateAuthorCommand, Guid>
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IUnitOfWork       _uow;
    private readonly IMapper           _mapper;

    public CreateAuthorHandler(
        IAuthorRepository authorRepo,
        IUnitOfWork       uow,
        IMapper           mapper)
    {
        _authorRepo = authorRepo;
        _uow        = uow;
        _mapper     = mapper;
    }

    public async Task<Guid> Handle(CreateAuthorCommand cmd, CancellationToken ct)
    {
        if (await _authorRepo.ExistsAsync(cmd.Request.FullName, ct))
            throw new InvalidOperationException($"Author '{cmd.Request.FullName}' already exists");

        var author = _mapper.Map<Author>(cmd.Request);
        _authorRepo.Add(author);
        await _uow.SaveChangesAsync(ct);
        return author.Id;
    }
}