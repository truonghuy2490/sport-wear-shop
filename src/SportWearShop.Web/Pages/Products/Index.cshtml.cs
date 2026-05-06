// Pages/Products/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductApiService _productApiService;

    public PagingResponseModel<ProductResponseModel>? Products { get; private set; }

    public IndexModel(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task OnGetAsync(
        int pageNumber = 1,
        int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        Products = await _productApiService.GetAllAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }
}