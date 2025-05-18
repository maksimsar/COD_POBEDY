namespace MetadataService.DTOs;

public sealed record UpdateTagRequest(
    string? Name,
    bool? Approved);