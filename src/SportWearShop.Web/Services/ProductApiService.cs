// SportWearShop.Web/Services/ProductApiClient.cs
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services
{
    public class ProductApiClient : ApiClient, IProductApiService
    {
        /*public ProductApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            return await GetAsync<List<ProductViewModel>>("api/products") ?? new List<ProductViewModel>();
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            return await GetAsync<ProductViewModel>($"api/products/{id}");
        }

        public async Task<PagingViewModel<ProductViewModel>> GetPagedProductsAsync(ProductQueryViewModel query)
        {
            // Build query string
            var queryString = BuildQueryString(query);
            return await GetAsync<PagingViewModel<ProductViewModel>>($"api/products{queryString}")
                   ?? new PagingViewModel<ProductViewModel>(new List<ProductViewModel>(), 0, 1, 10);
        }

        public async Task<ProductDetailViewModel?> GetProductDetailByIdAsync(long id)
        {
            return await GetAsync<ProductDetailViewModel>($"api/products/{id}/detail");
        }*/
        public ProductApiClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}