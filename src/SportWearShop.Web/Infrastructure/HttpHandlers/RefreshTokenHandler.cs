using System.Net;
using System.Net.Http.Headers;
using System.Text;
using SportWearShop.Shared.ViewModels.AuthModels;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Infrastructure.HttpHandlers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IAuthCookieService _authCookieService;
    private readonly IHttpClientFactory _httpClientFactory;

    public RefreshTokenHandler(
        IAuthCookieService authCookieService,
        IHttpClientFactory httpClientFactory
    )
    {
        _authCookieService = authCookieService;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (IsAuthEndpoint(request))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // clone send request
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content is not null)
        {
            var content = await request.Content.ReadAsStringAsync();

            clone.Content = new StringContent(
                content,
                Encoding.UTF8,
                request.Content.Headers.ContentType?.MediaType ?? "application/json");
        }

        // get access token and add to header value
        var accessToken = _authCookieService.GetAccessToken();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // first try
        var response = await base.SendAsync(request, cancellationToken);

        
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }
        
        // if expried or 401 do api/refresh-token 
        var refreshed = await TryRefreshTokenAsync(cancellationToken);

        if (!refreshed)
        {
            _authCookieService.ClearTokens();
            return response;
        }
        response.Dispose();

        // using new access token from cookies
        accessToken = _authCookieService.GetAccessToken();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            clone.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(clone, cancellationToken);
    }

    private async Task<bool> TryRefreshTokenAsync(
        CancellationToken cancellationToken)
    {
        var userId = _authCookieService.GetUserId();
        var refreshToken = _authCookieService.GetRefreshToken();

        if (userId is null || string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var client = _httpClientFactory.CreateClient("SportWearShopApiRaw");

        var request = new RefreshTokenRequestModel
        {
            UserId = userId.Value,
            RefreshToken = refreshToken
        };

        // call api do /refresh-token
        var response = await client.PostAsJsonAsync(
            "api/auth/refresh-token",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return false;

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseModel>(
            cancellationToken: cancellationToken);

        if (authResponse is null)
            return false;

        _authCookieService.SaveTokens(authResponse);

        return true;
    }

    private static bool IsAuthEndpoint(HttpRequestMessage request)
    {
        var path = request.RequestUri?.AbsolutePath.ToLowerInvariant();
        // these return refresh token avoid these make sure no loop in here, idk gpt said that XD
        return path is not null &&
               (
                   path.Contains("/api/auth/login") || 
                   path.Contains("/api/auth/register") || 
                   path.Contains("/api/auth/refresh-token")
               );
    }
}