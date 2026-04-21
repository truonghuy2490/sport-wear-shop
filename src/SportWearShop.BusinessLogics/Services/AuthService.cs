using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.AuthModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;

public class AuthService : IAuthService
{
    public Task<bool> AssignRoleAsync(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangePasswordAsync(ChangePasswordRequestModel request)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseModel> GetCurrentUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEmailExistAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseModel> LoginAsync(LoginRequestModel request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseModel> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseModel> RegisterAsync(RegisterRequestModel request)
    {
        throw new NotImplementedException();
    }
}