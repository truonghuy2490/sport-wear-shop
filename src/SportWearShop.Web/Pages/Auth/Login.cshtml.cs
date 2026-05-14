using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels.AuthModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAuthCookieService _authCookieService;
    public LoginModel(IAuthApiService authApiService, IAuthCookieService authCookieService)
    {
        _authApiService = authApiService;
        _authCookieService = authCookieService;
    }

    [BindProperty]
    public LoginRequestModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public IActionResult OnGet()
    {
        if (!string.IsNullOrWhiteSpace(_authCookieService.GetAccessToken()))
            return RedirectToPage("/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please check your email and password.";
            return Page();
        }

        try
        {
            var result = await _authApiService.LoginAsync(
                Input,
                cancellationToken);

            if (result is null)
            {
                ErrorMessage = "Email or password is incorrect.";
                return Page();
            }

            _authCookieService.SaveTokens(result);

            TempData["SuccessMessage"] = "Login successfully.";

            return RedirectToPage("/Index");
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorMessage = "Invalid login information.";
            return Page();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Email or password is incorrect.";
            return Page();
        }
        catch (ApiException)
        {
            ErrorMessage = "Login failed. Please try again later.";
            return Page();
        }
        catch
        {
            ErrorMessage = "Something went wrong. Please try again.";
            return Page();
        }
    }
}