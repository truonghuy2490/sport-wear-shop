using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Shared.ViewModels.ProductModels.ProductVarientModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products;

public class DetailsModel : PageModel
{
    private readonly IProductApiService _productApiService;

    public ProductDetailResponseModel? Product { get; private set; }

    public ProductVariantResponseModel? SelectedVariant { get; private set; }

    public List<ProductVariantResponseModel> AvailableColorVariants { get; private set; } = [];
    public List<ProductVariantResponseModel> AvailableSizeVariants { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? ColorCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SizeCode { get; set; }

    [TempData]
    public string? ToastMessage { get; set; }

    [TempData]
    public string? ToastType { get; set; }

    public DetailsModel(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task<IActionResult> OnGetAsync(
        long productId,
        CancellationToken cancellationToken)
    {
        try
        {
            Product = await _productApiService.GetByIdAsync(productId, cancellationToken);

            if (Product is null)
            {
                ToastMessage = "Product not found.";
                ToastType = "warning";
                return RedirectToPage("/Products/Index");
            }

            BuildVariantSelection();

            return Page();
        }
        catch (ApiException)
        {
            ToastMessage = "Cannot load product detail.";
            ToastType = "danger";
            return RedirectToPage("/Products/Index");
        }
        catch
        {
            ToastMessage = "Something went wrong.";
            ToastType = "danger";
            return RedirectToPage("/Products/Index");
        }
    }

    private void BuildVariantSelection()
    {
        var variants = Product!.Variants
            .Where(x => x.Status == "Active")
            .ToList();

        AvailableColorVariants = variants
            .GroupBy(x => x.ColorCode)
            .Select(g => g.First())
            .ToList();

        if (string.IsNullOrWhiteSpace(ColorCode) && AvailableColorVariants.Any())
        {
            ColorCode = AvailableColorVariants.First().ColorCode;
        }

        AvailableSizeVariants = variants
            .Where(x => x.ColorCode == ColorCode)
            .GroupBy(x => x.SizeCode)
            .Select(g => g.First())
            .ToList();

        if (!string.IsNullOrWhiteSpace(SizeCode))
        {
            SelectedVariant = variants.FirstOrDefault(x =>
                x.ColorCode == ColorCode &&
                x.SizeCode == SizeCode);

            if (SelectedVariant is null)
            {
                ToastMessage = "This color and size combination is not available.";
                ToastType = "warning";
                SizeCode = null;
            }
            else if (SelectedVariant.AvailableStock <= 0)
            {
                ToastMessage = "This variant is out of stock.";
                ToastType = "warning";
            }
        }

        SelectedVariant ??= AvailableSizeVariants.FirstOrDefault(x => x.AvailableStock > 0)
            ?? AvailableSizeVariants.FirstOrDefault();
    }
}