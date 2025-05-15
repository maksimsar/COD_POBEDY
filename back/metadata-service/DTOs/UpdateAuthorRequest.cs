namespace MetadataService.DTOs;

public sealed record UpdateAuthorRequest(
    string? FullName,
    short? BirthYear,
    short? DeathYear,
    string? Notes);