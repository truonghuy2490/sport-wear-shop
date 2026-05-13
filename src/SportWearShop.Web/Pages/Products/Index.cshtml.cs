using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductApiService _productApiService;

    public PagingResponseModel<ProductResponseModel>? Products { get; private set; }

    [BindProperty(SupportsGet = true)]
    public ProductQueryRequestModel Query { get; set; } = new();

    [TempData]
    public string? ToastMessage { get; set; }

    [TempData]
    public string? ToastType { get; set; }

    public IndexModel(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            NormalizeQuery();

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
        catch (Exception)
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
    }

    private PagingResponseModel<ProductResponseModel> CreateEmptyPagingResult()
    {
        return new PagingResponseModel<ProductResponseModel>(
            items: Enumerable.Empty<ProductResponseModel>(),
            totalCount: 0,
            pageNumber: Query.PageNumber,
            pageSize: Query.PageSize);
    }
}