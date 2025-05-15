namespace MetadataService.DTOs;

public sealed record CreateAuthorRequest(
    string FullName,
    short? BirthYear,
    short? DeathYear,
    string? Notes);