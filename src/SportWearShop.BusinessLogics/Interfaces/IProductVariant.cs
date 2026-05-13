using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IProductVariantService
{
    Task<PagingResponseModel<ProductVariantResponseModel>> GetByProductIdAsync(
        long productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductVariantDetailResponseModel?> GetByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

    Task<List<ProductVariantResponseModel>> CreateManyAsync(
        long productId,
        CreateProductVariantsRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductVariantResponseModel> UpdateAsync(
        long productVariantId,
        UpdateProductVariantRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

    Task<ProductVariantDetailResponseModel> UpdateSortOrdersAsync(
        long productVariantId,
        UpdateProductImageSortOrdersRequestModel request,
        CancellationToken cancellationToken = default);

    Task<AdminProductVariantDetailResponseModel> GetAdminByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);
}
