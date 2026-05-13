using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.BrandModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductApiService _productApiService;
    private readonly IBrandApiService _brandApiService;

    public PagingResponseModel<ProductResponseModel>? Products { get; private set; }

    public List<BrandResponseModel> Brands { get; private set; } = [];

    public BrandResponseModel? SelectedBrand { get; private set; }

    [BindProperty(SupportsGet = true)]
    public ProductQueryRequestModel Query { get; set; } = new();

    [TempData]
    public string? ToastMessage { get; set; }

    [TempData]
    public string? ToastType { get; set; }

    public IndexModel(
        IProductApiService productApiService,
        IBrandApiService brandApiService)
    {
        _productApiService = productApiService;
        _brandApiService = brandApiService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            NormalizeQuery();

            var brandResult = await _brandApiService.GetAllAsync(
                pageNumber: 1,
                pageSize: 100,
                cancellationToken);

            Brands = brandResult?.Items
                .Where(x => x.IsActive)
                .ToList() ?? [];

            if (Query.BrandIds?.Any() == true)
            {
                SelectedBrand = Brands.FirstOrDefault(x =>
                    x.BrandId == Query.BrandIds.First());
            }

            Products = await _productApiService.GetAllAsync(
                Query,
                cancellationToken);

            Products ??= CreateEmptyPagingResult();
        }
        catch (ApiException)
        {
            ToastMessage = "Cannot load products from API.";
            ToastType = "danger";
            Products = CreateEmptyPagingResult();
        }
        catch (HttpRequestException)
        {
            ToastMessage = "Cannot connect to API server.";
            ToastType = "warning";
            Products = CreateEmptyPagingResult();
        }
        catch
        {
            ToastMessage = "Something went wrong while loading products.";
            ToastType = "danger";
            Products = CreateEmptyPagingResult();
        }

        return Page();
    }

    private void NormalizeQuery()
    {
        if (Query.PageNumber <= 0)
            Query.PageNumber = 1;

        if (Query.PageSize <= 0)
            Query.PageSize = 12;
            
        if (Query.BrandIds?.Any() == true)
            Query.BrandIds = [Query.BrandIds.First()];
    }

    private PagingResponseModel<ProductResponseModel> CreateEmptyPagingResult()
    {
        return new PagingResponseModel<ProductResponseModel>(
            Enumerable.Empty<ProductResponseModel>(),
            0,
            Query.PageNumber,
            Query.PageSize);
    }
}