namespace AuthService.DTOs;

public record TokenResponse(
    string AccessToken,
    Guid   RefreshToken,
    DateTimeOffset ExpiresAt);