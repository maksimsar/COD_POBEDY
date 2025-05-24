using System.Net;
using System.Security.Claims;
using AuthService.Models;

namespace AuthService.Services;

public interface IJwtTokenService
{
    Task<(string Access, RefreshToken Refresh)> GenerateTokensAsync(User user,
        IEnumerable<string> roles,
        string userAgent,
        IPAddress ip,
        CancellationToken ct = default);
    
    ClaimsPrincipal? ValidateAccessToken(string jwt);
}