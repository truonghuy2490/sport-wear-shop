using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IProductService
{
    // --- QUERY ---

    // Product 
    Task<PagingResponseModel<ProductResponseModel>> GetPagedAsync(
        ProductQueryModel query,
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    /*Task<IEnumerable<ProductResponseModel>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default);

    // Rating
    Task<double> GetAverageRatingAsync(
       long productId,
       CancellationToken cancellationToken = default);

    // Inventory
    Task<int> GetTotalStockAsync(
        long productId,
        CancellationToken cancellationToken = default);*/

    // --- COMMAND ---

    // Products
    Task<long> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        long productId,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        long productId,
        CancellationToken cancellationToken = default);

    /*// Product Variants
    Task<long> AddProductVariantAsync(
        long productId,
        CreateProductVariantRequestModel request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateProductVariantAsync(
        long variantId,
        UpdateProductVariantRequestModel request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProductVariantAsync(
        long variantId,
        CancellationToken cancellationToken = default);

    // Product Images
    Task<long> AddProductImageAsync(
        CreateProductImageRequestModel request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProductImageAsync(
        long productImageId,
        CancellationToken cancellationToken = default);

    // Product Ratings
    Task<bool> AddProductRatingAsync(
        CreateProductRatingRequestModel request,
        CancellationToken cancellationToken = default);*/

   
}