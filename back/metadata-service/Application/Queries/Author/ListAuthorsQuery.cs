using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Author;

public sealed record ListAuthorsQuery() : IRequest<IReadOnlyList<AuthorDto>>;