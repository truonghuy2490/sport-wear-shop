

using SportWearShop.BusinessLogics.ResponseModels.AuthModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IAuthService
{
    // Register Async
    Task<AuthResponseModel> RegisterAsync(
        RegisterRequestModel request,
        CancellationToken cancellationToken = default);
        
    // Login Async
    Task<AuthResponseModel> LoginAsync(
        LoginRequestModel request,
        CancellationToken cancellationToken = default);

    // Logout Async
    Task LogoutAsync(
        long userId, // next update using JWT claim id
        CancellationToken cancellationToken = default);

    // Refresh Token Async 
    Task<AuthResponseModel> RefreshTokenAsync(
        RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default);
}