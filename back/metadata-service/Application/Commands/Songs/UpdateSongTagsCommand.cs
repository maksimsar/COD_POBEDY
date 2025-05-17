using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record UpdateSongTagsCommand(UpdateTagsRequest Request) : IRequest;