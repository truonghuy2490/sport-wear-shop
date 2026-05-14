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
        ProductQueryRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default);
}