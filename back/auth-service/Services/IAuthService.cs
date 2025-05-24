using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<TokenResponse> RegisterAsync(string email, string password, string fullName, CancellationToken ct = default);

    Task<TokenResponse> LoginAsync(string email, string password, CancellationToken ct = default);

    Task<TokenResponse> RefreshAsync(Guid refreshToken, string userAgent, string ip, CancellationToken ct = default);
}