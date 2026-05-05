using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;

namespace SportWearShop.BusinessLogics.Interfaces;
public interface IProductImageService
{
    Task<List<ProductImageResponseModel>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<List<ProductImageResponseModel>> GetByVariantIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

    Task<ProductImageResponseModel> CreateAsync(
        CreateProductImageRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductImageResponseModel> UpdateAsync(
        long productImageId,
        UpdateProductImageRequestModel request,
        CancellationToken cancellationToken = default);

    Task SetPrimaryAsync(
        long productImageId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long productImageId,
        CancellationToken cancellationToken = default);
}