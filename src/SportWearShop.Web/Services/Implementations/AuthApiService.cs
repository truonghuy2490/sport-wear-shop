using SportWearShop.Shared.ViewModels.AuthModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class AuthApiService : IAuthApiService
{
    private readonly ApiClient _apiClient;

    public AuthApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<AuthResponseModel?> RegisterAsync(
        RegisterRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<RegisterRequestModel, AuthResponseModel>(
            "api/auth/register",
            request,
            cancellationToken);
    }

    public async Task<AuthResponseModel?> LoginAsync(
        LoginRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<LoginRequestModel, AuthResponseModel>(
            "api/auth/login",
            request,
            cancellationToken);
    }

    public async Task<AuthResponseModel?> RefreshTokenAsync(
        RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<RefreshTokenRequestModel, AuthResponseModel>(
            "api/auth/refresh-token",
            request,
            cancellationToken);
    }

    public async Task LogoutAsync(
        CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync<object, object?>(
            "api/auth/logout",
            new { },
            cancellationToken);
    }
}