

using SportWearShop.BusinessLogics.ResponseModels.AuthModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IAuthService
{
    // Register
    Task<AuthResponseModel> RegisterAsync(RegisterRequestModel request);

    // Login
    Task<AuthResponseModel> LoginAsync(LoginRequestModel request);

    // Refresh Token
    Task<AuthResponseModel> RefreshTokenAsync(string refreshToken);

    // Get current user info
    Task<UserResponseModel> GetCurrentUserAsync(string userId);

    // Assign role
    Task<bool> AssignRoleAsync(string userId, string role);

    // Change password
    Task<bool> ChangePasswordAsync(ChangePasswordRequestModel request);

    // Check email exist
    Task<bool> IsEmailExistAsync(string email);
}