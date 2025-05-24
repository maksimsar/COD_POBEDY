using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;
    
    [HttpPost("register")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest dto,
        CancellationToken           ct)
    {
        if (!ModelState.IsValid)  
            return ValidationProblem(ModelState);

        try
        {
            var tokens = await _auth.RegisterAsync(
                dto.Email.Trim(), dto.Password, dto.FullName.Trim(), ct);

            return Created(string.Empty, tokens); 
        }
        catch (InvalidOperationException ex) 
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    

    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest dto,
        CancellationToken        ct)
    {
        try
        {
            var tokens = await _auth.LoginAsync(dto.Email.Trim(), dto.Password, ct);
            return Ok(tokens);   
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest dto,
        CancellationToken          ct)
    {
        try
        {
            var ua  = Request.Headers.UserAgent.ToString();
            var ip  = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "-";

            var tokens = await _auth.RefreshAsync(dto.RefreshToken, ua, ip, ct);
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
