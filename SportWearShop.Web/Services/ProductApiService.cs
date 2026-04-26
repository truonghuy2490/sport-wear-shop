// SportWearShop.Web/Services/ProductApiClient.cs
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services
{
    public class ProductApiClient : ApiClient, IProductApiService
    {
        public ProductApiClient(HttpClient httpClient) : base(httpClient)
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
        }

        /*public async Task<bool> CreateProductAsync(CreateProductRequestModel product)
        {
            var result = await PostAsync<CreateProductRequestModel, ProductResponseModel>(
                "api/products", product);
            return result != null;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductRequestModel product)
        {
            var result = await PutAsync<UpdateProductRequestModel, ProductResponseModel>(
                $"api/products/{product.ProductId}", product);
            return result != null;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await DeleteAsync($"api/products/{id}");
        }*/

        private string BuildQueryString(ProductQueryViewModel query)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
                parameters.Add($"keyword={Uri.EscapeDataString(query.Keyword)}");

            if (query.BrandId.HasValue)
                parameters.Add($"brandId={query.BrandId}");

            if (query.CategoryId.HasValue)
                parameters.Add($"categoryId={query.CategoryId}");

            if (!string.IsNullOrWhiteSpace(query.Gender))
                parameters.Add($"gender={query.Gender}");

            if (query.MinPrice.HasValue)
                parameters.Add($"minPrice={query.MinPrice}");

            if (query.MaxPrice.HasValue)
                parameters.Add($"maxPrice={query.MaxPrice}");

            parameters.Add($"pageNumber={query.PageNumber}");
            parameters.Add($"pageSize={query.PageSize}");

            return parameters.Any() ? "?" + string.Join("&", parameters) : "";
        }
    }
}