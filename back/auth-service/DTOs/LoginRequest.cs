namespace AuthService.DTOs;

public sealed record LoginRequest(string Email, string Password);