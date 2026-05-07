using SportWearShop.Shared.ViewModels.AuthModels;

namespace SportWearShop.Web.Services.Interfaces;

public interface IAuthCookieService
{
    string? GetAccessToken();

    string? GetRefreshToken();

    long? GetUserId();

    void SaveTokens(AuthResponseModel response);

    void ClearTokens();
}