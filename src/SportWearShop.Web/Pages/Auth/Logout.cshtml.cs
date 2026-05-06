using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportWearShop.Web.Pages.Auth;

public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        return RedirectToPage("/Index");
    }
}