using System.Net.Http.Headers;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Infrastructure.HttpHandlers;
// using refresh token handler instead of delegate access token handler 
/*
public class AccessTokenHandler : DelegatingHandler
{
    private readonly IAuthCookieService _authCookieService;

    public AccessTokenHandler(
        IAuthCookieService authCookieService
    )
    {
        _authCookieService = authCookieService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = _authCookieService.GetAccessToken();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue( "Bearer", accessToken);
        }

        return await base.SendAsync( request, cancellationToken);
    }
}
*/