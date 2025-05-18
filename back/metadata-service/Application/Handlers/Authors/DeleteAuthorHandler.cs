using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MetadataService.Application.Commands;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

/// <summary>
/// Обработчик команды DeleteAuthorCommand.
/// </summary>
internal sealed class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand, Unit>
{
    private readonly IAuthorRepository _authorRepo;
    private readonly ISongRepository   _songRepo;
    private readonly IUnitOfWork       _uow;

    public DeleteAuthorHandler(
        IAuthorRepository authorRepo,
        ISongRepository   songRepo,
        IUnitOfWork       uow)
    {
        _authorRepo = authorRepo;
        _songRepo   = songRepo;
        _uow        = uow;
    }

    public async Task<Unit> Handle(DeleteAuthorCommand cmd, CancellationToken ct)
    {
        var author = await _authorRepo.GetByIdAsync(cmd.Id, ct)
                     ?? throw new KeyNotFoundException($"Author {cmd.Id} not found");

        // Проверяем, что автор не связан с песнями
        var linked = await _songRepo.ListAsync(
            s => s.SongAuthors.Any(sa => sa.AuthorId == cmd.Id), ct);
        if (linked.Any())
            throw new InvalidOperationException("Cannot delete author with linked songs");

        _authorRepo.Remove(author);
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}