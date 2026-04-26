// SportWearShop.Web/Services/Interfaces/IProductMockService.cs
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;

namespace SportWearShop.Web.Services.Interfaces
{
    public interface IProductMockService
    {
        Task<PagingViewModel<ProductViewModel>> GetPagedProductsAsync(ProductQueryViewModel query);
        Task<ProductDetailViewModel> GetProductByIdAsync(long id);
        Task<ProductDetailViewModel> GetProductBySlugAsync(string slug);
        Task<List<ProductViewModel>> GetProductsByBrandAsync(int brandId, int limit = 10);
        Task<List<ProductViewModel>> GetRelatedProductsAsync(long productId, int limit = 5);
    }
}