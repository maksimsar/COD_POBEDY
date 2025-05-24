using System.Net;
using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<TokenResponse> RegisterAsync(string email, string password, string fullName, string userAgent, IPAddress ipAddress, CancellationToken ct = default);

    Task<TokenResponse> LoginAsync(string email, string password, string userAgent, IPAddress ipAddress, CancellationToken ct = default);

    Task<TokenResponse> RefreshAsync(Guid refreshToken, string userAgent, IPAddress ip, CancellationToken ct = default);
}