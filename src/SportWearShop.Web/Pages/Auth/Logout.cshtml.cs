using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAuthCookieService _authCookieService;

    public LogoutModel(
        IAuthApiService authApiService,
        IAuthCookieService authCookieService)
    {
        _authApiService = authApiService;
        _authCookieService = authCookieService;
    }

    public async Task<IActionResult> OnPostAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await _authApiService.LogoutAsync(cancellationToken);
        }
        catch (ApiException)
        {
            // API logout failed, but still clear local cookies
            // because user expects to be logged out from this browser.
        }
        catch (Exception)
        {
            // Same reason: local logout should still continue.
        }
        finally
        {
            _authCookieService.ClearTokens();
        }

        TempData["SuccessMessage"] = "Logout successfully.";

        return RedirectToPage("/Index");
    }
}