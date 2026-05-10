// Pages/Products/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD
=======
=======
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
    }
}