using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Tags;

public sealed record ListTagsQuery() : IRequest<IReadOnlyList<TagDto>>;