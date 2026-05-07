using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IAuthCookieService _authCookieService;

    public CurrentUserService(
        IAuthCookieService authCookieService)
    {
        _authCookieService = authCookieService;
    }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(
            _authCookieService.GetAccessToken());

    public long? UserId
    {
        get
        {
            var value =
                GetClaimValue(ClaimTypes.NameIdentifier)
                ?? GetClaimValue(JwtRegisteredClaimNames.Sub)
                ?? GetClaimValue("userId");

            if (long.TryParse(value, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    public string? Email =>
        GetClaimValue(ClaimTypes.Email)
        ?? GetClaimValue(JwtRegisteredClaimNames.Email)
        ?? GetClaimValue("email");

    public string? Name =>
        GetClaimValue(ClaimTypes.Name)
        ?? GetClaimValue("name")
        ?? GetClaimValue("firstName")
        ?? GetClaimValue("first_name");

    public string DisplayName =>
        Name
        ?? Email
        ?? "Customer";

    private string? GetClaimValue(string claimType)
    {
        var accessToken =
            _authCookieService.GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(accessToken);

            return jwt.Claims
                .FirstOrDefault(x => x.Type == claimType)?
                .Value;
        }
        catch
        {
            return null;
        }
    }
}