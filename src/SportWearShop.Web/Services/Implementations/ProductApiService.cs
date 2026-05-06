
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class ProductApiService : IProductApiService
{
    private readonly ApiClient _apiClient;

    public ProductApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PagingResponseModel<ProductResponseModel>?> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<PagingResponseModel<ProductResponseModel>>(
            $"api/products?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
    }

    public async Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<ProductDetailResponseModel>(
            $"api/products/{productId}",
            cancellationToken);
    }

    public async Task<ProductResponseModel?> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<CreateProductRequestModel, ProductResponseModel>(
            "api/products",
            request,
            cancellationToken);
    }

    public async Task<ProductResponseModel?> UpdateAsync(
        long productId,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PutAsync<UpdateProductRequestModel, ProductResponseModel>(
            $"api/products/{productId}",
            request,
            cancellationToken);
    }

    public async Task DeleteAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        await _apiClient.DeleteAsync(
            $"api/products/{productId}",
            cancellationToken);
    }
}