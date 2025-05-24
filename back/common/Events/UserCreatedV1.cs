namespace Common.Events;

public sealed record UserCreatedV1(
    Guid   UserId,
    string Email,
    string FullName);