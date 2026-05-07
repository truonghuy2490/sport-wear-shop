using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SportWearShop.Shared.ViewModels.AuthModels;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class AuthCookieService : IAuthCookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Cookies["AccessToken"];
    }

    public string? GetRefreshToken()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Cookies["RefreshToken"];
    }
    public long? GetUserId()
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(accessToken);

            var userIdClaim =
                jwt.Claims.FirstOrDefault(
                    x => x.Type == ClaimTypes.NameIdentifier)?.Value
                ?? jwt.Claims.FirstOrDefault(
                    x => x.Type == JwtRegisteredClaimNames.Sub)?.Value
                ?? jwt.Claims.FirstOrDefault(
                    x => x.Type == "userId")?.Value;

            if (long.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public void SaveTokens(AuthResponseModel response)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        httpContext.Response.Cookies.Append(
            "AccessToken",
            response.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = response.AccessTokenExpiresAtUtc
            });

        if (!string.IsNullOrWhiteSpace(response.RefreshToken))
        {
            httpContext.Response.Cookies.Append(
                "RefreshToken",
                response.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = response.RefreshTokenExpiresAtUtc
                });
        }
    }

    public void ClearTokens()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        httpContext.Response.Cookies.Delete("AccessToken");
        httpContext.Response.Cookies.Delete("RefreshToken");
    }
}