using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.AuthModels;

namespace SportWearShop.APIs.Controllers;

/*
-- next update --
POST /api/auth/register:        v
POST /api/auth/login:           v
POST /api/auth/google-login
POST /api/auth/refresh-token:   v (basic)
POST /api/auth/logout:          v (basic)
GET  /api/auth/me
-- later --
POST /api/auth/forgot-password
POST /api/auth/reset-password
POST /api/auth/confirm-email
*/

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/auth/refresh-token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.RefreshTokenAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync(
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        await _authService.LogoutAsync(
            userId,
            cancellationToken);

        return NoContent();
    }
}