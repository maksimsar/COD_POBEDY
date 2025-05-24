using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Configuration;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings               _opt;
    private readonly IUserRepository           _users;
    private readonly IRefreshTokenRepository   _refresh;
    private readonly SymmetricSecurityKey      _key;
    private readonly SigningCredentials        _creds;
    private readonly JwtSecurityTokenHandler   _handler = new();

    public JwtTokenService(
        IOptions<JwtSettings>     opt,
        IUserRepository           users,
        IRefreshTokenRepository   refresh)
    {
        _opt     = opt.Value;
        _users   = users;
        _refresh = refresh;

        _key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
        _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
    }
    

    public async Task<(string Access, RefreshToken Refresh)> GenerateTokensAsync(
        User user,
        IEnumerable<string> roles,
        string userAgent,
        string ip,
        CancellationToken ct = default)
    {
        /* 1. Access-token (~15 минут) */
        var now   = DateTimeOffset.UtcNow;
        var jwt   = new JwtSecurityToken(
            issuer:   _opt.Issuer,
            audience: _opt.Audience,
            notBefore: now.UtcDateTime,
            expires:   now.AddMinutes(_opt.AccessLife).UtcDateTime,
            claims: BuildClaims(user, roles),
            signingCredentials: _creds);

        string accessToken = _handler.WriteToken(jwt);

        /* 2. Refresh-token (Guid + база) */
        var refresh = new RefreshToken
        {
            Token       = Guid.NewGuid(),
            UserId      = user.Id,
            ExpiresAt   = now.AddDays(_opt.RefreshLife),
            IssuedAt    = now,
            UserAgent   = userAgent.Truncate(100),
            IpAddress   = ip
        };

        _refresh.Add(refresh);
        await _refresh.SaveAsync(ct);

        return (accessToken, refresh);
    }

    public ClaimsPrincipal? ValidateAccessToken(string jwt)
    {
        try
        {
            var prm = new TokenValidationParameters
            {
                IssuerSigningKey      = _key,
                ValidIssuer           = _opt.Issuer,
                ValidAudience         = _opt.Audience,
                ValidateIssuer        = true,
                ValidateAudience      = true,
                ValidateLifetime      = true,
                ClockSkew             = TimeSpan.FromSeconds(30)
            };

            return _handler.ValidateToken(jwt, prm, out _);
        }
        catch { return null; }
    }
    

    private static IEnumerable<Claim> BuildClaims(User u, IEnumerable<string> roles)
    {
        yield return new Claim(JwtRegisteredClaimNames.Sub,   u.Id.ToString());
        yield return new Claim(JwtRegisteredClaimNames.Email, u.Email);
        yield return new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString());

        foreach (var role in roles)
            yield return new Claim(ClaimTypes.Role, role);
    }
}

/* маленький extension, чтобы не выбросить слишком длинный user-agent */
file static class StringExt
{
    public static string Truncate(this string s, int max) =>
        s.Length <= max ? s : s[..max];
}