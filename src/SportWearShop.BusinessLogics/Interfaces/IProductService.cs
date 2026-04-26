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
    // --- TEST ---
    /// <summary>
    /// Gets paged list of products with filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged response with product list</returns>
    Task<PagingResponseModel<ProductResponseModel>> GetPagedAsync(
        ProductQueryModel query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product details by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed product information</returns>
    /// <exception cref="NotFoundException">Thrown when product not found</exception>
    Task<ProductDetailResponseModel> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product details by slug
    /// </summary>
    /// <param name="slug">Product slug (URL-friendly name)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed product information</returns>
    /// <exception cref="NotFoundException">Thrown when product not found</exception>
    Task<ProductDetailResponseModel> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product information</returns>
    /// <exception cref="NotFoundException">Thrown when brand or category not found</exception>
    /// <exception cref="ConflictException">Thrown when product code or slug already exists</exception>
    Task<ProductResponseModel> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="request">Updated product data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product information</returns>
    /// <exception cref="NotFoundException">Thrown when product, brand, or category not found</exception>
    /// <exception cref="ConflictException">Thrown when product code or slug already exists</exception>
    Task<ProductResponseModel> UpdateAsync(
        long id,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID to delete</param>
    /// <param name="softDelete">If true, only marks as inactive; if false, permanently removes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="NotFoundException">Thrown when product not found</exception>
    Task DeleteAsync(
        long id,
        bool softDelete = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product exists by ID
    /// </summary>
    /// <param name="id">Product ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product exists, false otherwise</returns>
    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product exists by product code
    /// </summary>
    /// <param name="productCode">Product code to check</param>
    /// <param name="excludeId">Optional product ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product exists, false otherwise</returns>
    Task<bool> ExistsByCodeAsync(
        string productCode,
        long? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="status">New status (e.g., "Active", "Inactive", "Draft")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product information</returns>
    /// <exception cref="NotFoundException">Thrown when product not found</exception>
    Task<ProductResponseModel> UpdateStatusAsync(
        long id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by brand
    /// </summary>
    /// <param name="brandId">Brand ID</param>
    /// <param name="limit">Maximum number of products to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of products from the specified brand</returns>
    Task<IEnumerable<ProductResponseModel>> GetProductsByBrandAsync(
        int brandId,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets related products based on category and brand
    /// </summary>
    /// <param name="productId">Product ID to find related products for</param>
    /// <param name="limit">Maximum number of related products to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of related products</returns>
    Task<IEnumerable<ProductResponseModel>> GetRelatedProductsAsync(
        long productId,
        int limit = 5,
        CancellationToken cancellationToken = default);
    // ------------
    /*
    // --- QUERY ---

    // Product 
    Task<PagingResponseModel<ProductResponseModel>> GetPagedAsync(
        ProductQueryModel query,
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductResponseModel>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default);

    // Rating
    Task<double> GetAverageRatingAsync(
       long productId,
       CancellationToken cancellationToken = default);

    // Inventory
    Task<int> GetTotalStockAsync(
        long productId,
        CancellationToken cancellationToken = default);

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

    // Product Variants
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