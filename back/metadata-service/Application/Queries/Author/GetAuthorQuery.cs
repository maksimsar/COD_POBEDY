using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Author;

public sealed record GetAuthorQuery(Guid Id) : IRequest<AuthorDto>;