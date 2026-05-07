using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IProductVariantService
{
    Task<List<ProductVariantResponseModel>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<ProductVariantDetailResponseModel?> GetByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

    Task<ProductVariantResponseModel> CreateAsync(
        long productId,
        CreateProductVariantRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductVariantResponseModel> UpdateAsync(
        long productVariantId,
        UpdateProductVariantRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

<<<<<<< HEAD
    Task<ProductVariantDetailResponseModel> UpdateSortOrdersAsync(
        long productVariantId,
        UpdateProductImageSortOrdersRequestModel request,
        CancellationToken cancellationToken = default);

=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
}
