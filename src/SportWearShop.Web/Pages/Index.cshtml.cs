<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;
using System.Net;

namespace SportWearShop.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IProductApiService _productApiService;

    public PagingResponseModel<ProductResponseModel>? FeaturedProducts { get; private set; }

    public string? ErrorMessage { get; private set; }

    public IndexModel(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            FeaturedProducts = await _productApiService.GetAllAsync(
                pageNumber: 1,
                pageSize: 8,
                cancellationToken: cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "You are not authorized. Please login again.";
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            ErrorMessage = "You do not have permission to view this content.";
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "Featured products not found.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Cannot connect to API server. Please make sure backend API is running.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Request timeout. Please try again.";
        }
        catch (Exception)
        {
            ErrorMessage = "Something went wrong while loading featured products.";
        }
    }
<<<<<<< HEAD
}
=======
}
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportWearShop.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
