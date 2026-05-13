using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;

namespace SportWearShop.Web.Services.Interfaces;

/// <summary>
/// product api service
/// </summary>
/// 
public interface IProductApiService
{
    Task<PagingResponseModel<ProductResponseModel>?> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<ProductResponseModel?> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductResponseModel?> UpdateAsync(
        long productId,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long productId,
        CancellationToken cancellationToken = default);
}
