using System.Net;
using System.Text.Json;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;
using Common.Events;
using Microsoft.AspNetCore.Identity;
using MassTransit;

namespace AuthService.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository           _users;
    private readonly IRoleRepository           _roles;
    private readonly IRefreshTokenRepository   _rtRepo;
    private readonly IJwtTokenService          _jwt;
    private readonly IPasswordHasher<User>     _hasher;
    private readonly IPublishEndpoint          _bus;
    private readonly ILogger<AuthService>      _log;

    public AuthService(IUserRepository         users,
                       IRoleRepository         roles,
                       IRefreshTokenRepository rtRepo,
                       IJwtTokenService        jwt,
                       IPasswordHasher<User>   hasher,
                       IPublishEndpoint        bus,
                       ILogger<AuthService>    log)
    {
        _users   = users;
        _roles   = roles;
        _rtRepo  = rtRepo;
        _jwt     = jwt;
        _hasher  = hasher;
        _bus     = bus;
        _log     = log;
    }

    /*──────────────────────────  REGISTER  ──────────────────────────*/

    public async Task<TokenResponse> RegisterAsync(string email, string password, string fullName, string userAgent, IPAddress ipAddr, CancellationToken ct = default)
    {
        // 1. Проверяем уникальность e-mail
        if (await _users.GetByEmailAsync(email, ct) is not null)
            throw new InvalidOperationException("User already exists");

        // 2. Хэшим пароль
        var user = new User
        {
            Id           = Guid.NewGuid(),
            Email        = email,
            FullName     = fullName,
            PasswordHash = _hasher.HashPassword(null!, password),
            CreatedAt    = DateTimeOffset.UtcNow,
            IsActive     = true
        };

        // 3. По умолчанию всем даём роль "User"
        var roleUser = await _roles.EnsureRoleAsync("User", ct);
        user.Roles.Add(new UserRole { RoleId = roleUser.Id });

        _users.Add(user);
        await _users.SaveAsync(ct);                     // INSERT users + user_roles

        // 4. Публикуем событие UserCreated → Outbox
        var evt = new UserCreatedV1(user.Id, user.Email, user.FullName);
        _users.AddOutbox(new OutboxMessage
        {
            Type          = typeof(UserCreatedV1).AssemblyQualifiedName!,
            Payload       = JsonSerializer.Serialize(evt),
            OccurredOnUtc = DateTimeOffset.UtcNow,
            Processed     = false
        });
        await _users.SaveAsync(ct);                     // INSERT outbox_messages

        // 5. Генерируем пару токенов
        var (access, refresh) = await _jwt.GenerateTokensAsync(
            user,
            new[] { "User" },
            userAgent,
            ipAddr,
            ct);

        return new TokenResponse(access, refresh.Token, refresh.ExpiresAt);
    }

    /*──────────────────────────  LOGIN  ────────────────────────────*/

    public async Task<TokenResponse> LoginAsync(string email, string password, string userAgent, IPAddress ipAddr, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials");

        // Проверка пароля
        var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (res == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials");

        // Роли понадобятся для claims
        var roles = user.Roles.Select(r => r.Role.Name).ToArray();

        var (access, refresh) = await _jwt.GenerateTokensAsync(
            user,
            roles,
            userAgent,
            ipAddr,
            ct);

        return new TokenResponse(access, refresh.Token, refresh.ExpiresAt);
    }

    /*──────────────────────────  REFRESH  ──────────────────────────*/

    public async Task<TokenResponse> RefreshAsync(
        Guid refreshToken, string userAgent, IPAddress  ip, CancellationToken ct = default)
    {
        var token = await _rtRepo.GetAsync(refreshToken, ct)
                    ?? throw new UnauthorizedAccessException("Token not found");

        if (token.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Token expired");

        var user = await _users.GetAsync(token.UserId, ct)
                   ?? throw new UnauthorizedAccessException("User missing");

        // Инвалидируем старый токен
        await _rtRepo.InvalidateAsync(refreshToken, ct);
        await _rtRepo.SaveAsync(ct);

        var roles = user.Roles.Select(r => r.Role.Name).ToArray();
        var (access, refresh) = await _jwt.GenerateTokensAsync(
            user, roles, userAgent, ip, ct);

        return new TokenResponse(access, refresh.Token, refresh.ExpiresAt);
    }
}