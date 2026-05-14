using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels.AuthModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAuthCookieService _authCookieService;

    public RegisterModel(IAuthApiService authApiService, IAuthCookieService auhCookieService)
    {
        _authApiService = authApiService;
        _authCookieService = auhCookieService;
    }

    [BindProperty]
    public RegisterRequestModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (Request.Cookies.ContainsKey("AccessToken"))
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please check your registration information.";
            return Page();
        }

        try
        {
            var result = await _authApiService.RegisterAsync(
                Input,
                cancellationToken);

            if (result is null)
            {
                ErrorMessage = "Register failed. Please try again.";
                return Page();
            }

            _authCookieService.SaveTokens(result);

            TempData["SuccessMessage"] = "Account created successfully.";

            return RedirectToPage("/Index");
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ErrorMessage = ex.ErrorResponse?.Message ?? "Invalid registration information.";
            return Page();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            ErrorMessage = "This email is already registered.";
            return Page();
        }
        catch (ApiException)
        {
            ErrorMessage = "Register failed. Please try again later.";
            return Page();
        }
        catch
        {
            ErrorMessage = "Something went wrong. Please try again.";
            return Page();
        }
    }
}