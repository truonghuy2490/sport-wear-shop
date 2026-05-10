using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products;

public class DetailsModel : PageModel
{
    private readonly IProductApiService _productApiService;

    public ProductDetailResponseModel? Product { get; private set; }

    public DetailsModel(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task<IActionResult> OnGetAsync(
        string slug,
        long productId,
        CancellationToken cancellationToken)
    {
        Product = await _productApiService.GetByIdAsync(
            productId,
            cancellationToken);

        if (Product == null)
        {
            return RedirectToPage("/Errors/StatusCode", new { statusCode = 404 });
        }

        // Nếu slug trên URL không đúng slug thật thì redirect về URL chuẩn
        if (!string.Equals(slug, Product.Slug, StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage("/Products/Details", new
            {
                slug = Product.Slug,
                productId = Product.ProductId
            });
        }

        return Page();
    }
}