// Pages/Products/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductMockService _productService;

        public IndexModel(IProductMockService productService)
        {
            _productService = productService;
        }

        public PagingViewModel<ProductViewModel> Products { get; set; }
        public ProductQueryViewModel Query { get; set; } = new();

        public async Task OnGetAsync(
            string? keyword = null,
            int? brandId = null,
            int? categoryId = null,
            string? gender = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Query = new ProductQueryViewModel
            {
                Keyword = keyword,
                BrandId = brandId,
                CategoryId = categoryId,
                Gender = gender,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Products = await _productService.GetPagedProductsAsync(Query);
        }
    }
}