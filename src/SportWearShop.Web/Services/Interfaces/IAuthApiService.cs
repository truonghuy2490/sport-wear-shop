using SportWearShop.Shared.ViewModels.AuthModels;

namespace SportWearShop.Web.Services.Interfaces;

public interface IAuthApiService
{
    Task<AuthResponseModel?> RegisterAsync(
        RegisterRequestModel request,
        CancellationToken cancellationToken = default);

    Task<AuthResponseModel?> LoginAsync(
        LoginRequestModel request,
        CancellationToken cancellationToken = default);

    Task<AuthResponseModel?> RefreshTokenAsync(
        RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        CancellationToken cancellationToken = default);
}