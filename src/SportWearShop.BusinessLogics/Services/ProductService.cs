using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace SportWearShop.BusinessLogics.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagingResponseModel<ProductResponseModel>> GetPagedAsync(
            ProductQueryModel query,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Product> productQuery = _unitOfWork.Products.Query();

            // Apply filters
            productQuery = ApplyFilters(productQuery, query);

            int totalCount = await productQuery.CountAsync(cancellationToken);
            int pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            int pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            productQuery = productQuery
                .OrderByDescending(p => p.UpdatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            List<ProductResponseModel> items = await productQuery
                .Select(p => new ProductResponseModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Slug = p.Slug,
                    Description = p.Description,
                    Gender = p.Gender,
                    BaseMaterial = p.BaseMaterial,
                    Status = p.Status,
                    BrandName = p.Brand.BrandName,
                    CategoryName = p.Category.CategoryName,
                    MinPrice = p.ProductVariants
                        .Select(v => v.SalePrice )
                        .Min() ?? 0,
                    MaxPrice = p.ProductVariants
                        .Select(v => (decimal?)v.ListPrice)
                        .Max() ?? 0,
                    ThumbnailUrl = p.ProductImages
                        .OrderBy(i => i.IsPrimary)
                        .ThenBy(i => i.SortOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    AverageRating = p.ProductRatings
                        .Select(r => (double?)r.RatingValue)
                        .Average() ?? 0,
                    TotalStock = p.ProductVariants
                        .Sum(v => v.InventoryStock != null
                            ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                            : 0)
                })
                .ToListAsync(cancellationToken);

            return new PagingResponseModel<ProductResponseModel>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<ProductDetailResponseModel> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product by id {ProductId}", id);

            var product = await _unitOfWork.Products
                .Query()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.InventoryStock)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductRatings)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            return MapToProductDetailResponse(product);
        }

        public async Task<ProductDetailResponseModel> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product by slug {Slug}", slug);

            var product = await _unitOfWork.Products
                .Query()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.InventoryStock)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductRatings)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with slug '{slug}' not found");
            }

            return MapToProductDetailResponse(product);
        }

        public async Task<ProductResponseModel> CreateAsync(
            CreateProductRequestModel request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new product: {ProductName}", request.ProductName);

            // Validate brand exists
            var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand == null)
            {
                throw new NotFoundException($"Brand with id {request.BrandId} not found");
            }

            // Validate category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with id {request.CategoryId} not found");
            }

            // Check for duplicate product code
            var existingProduct = await _unitOfWork.Products
                .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode, cancellationToken: cancellationToken);

            if (existingProduct != null)
            {
                throw new ConflictException($"Product with code '{request.ProductCode}' already exists");
            }

            // Generate slug if not provided
            string slug = string.IsNullOrWhiteSpace(request.Slug)
                ? GenerateSlug(request.ProductName)
                : request.Slug;

            // Check duplicate slug
            var slugExists = await _unitOfWork.Products
                .AnyAsync(p => p.Slug == slug, cancellationToken);

            if (slugExists)
            {
                throw new ConflictException($"Product with slug '{slug}' already exists");
            }

            var product = new Product
            {
                ProductName = request.ProductName,
                ProductCode = request.ProductCode,
                Slug = slug,
                Description = request.Description,
                Gender = request.Gender,
                BaseMaterial = request.BaseMaterial,
                Status = request.Status ?? "Active",
                BrandId = request.BrandId,
                CategoryId = request.CategoryId,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product created successfully with id {ProductId}", product.ProductId);

            // Return the created product
            return await GetProductResponseByIdAsync(product.ProductId, cancellationToken);
        }

        public async Task<ProductResponseModel> UpdateAsync(
            long id,
            UpdateProductRequestModel request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating product with id {ProductId}", id);

            var product = await _unitOfWork.Products
                .GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            // Validate brand if changed
            if (request.BrandId != null
                && request.BrandId != product.BrandId)
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId, cancellationToken);
                if (brand == null)
                {
                    throw new NotFoundException($"Brand with id {request.BrandId} not found");
                }
                product.BrandId = request.BrandId.Value;
            }

            // Validate category if changed
            if (request.CategoryId != null && request.CategoryId != product.CategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
                if (category == null)
                {
                    throw new NotFoundException($"Category with id {request.CategoryId} not found");
                }
                product.CategoryId = request.CategoryId.Value;
            }

            // Check product code uniqueness if changed
            if (!string.IsNullOrWhiteSpace(request.ProductCode) && request.ProductCode != product.ProductCode)
            {
                var existingProduct = await _unitOfWork.Products
                    .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode, cancellationToken: cancellationToken);

                if (existingProduct != null)
                {
                    throw new ConflictException($"Product with code '{request.ProductCode}' already exists");
                }
                product.ProductCode = request.ProductCode;
            }

            // Check slug uniqueness if changed
            if (!string.IsNullOrWhiteSpace(request.Slug) && request.Slug != product.Slug)
            {
                var slugExists = await _unitOfWork.Products
                    .AnyAsync(p => p.Slug == request.Slug && p.ProductId != id, cancellationToken);

                if (slugExists)
                {
                    throw new ConflictException($"Product with slug '{request.Slug}' already exists");
                }
                product.Slug = request.Slug;
            }

            // Update properties
            product.ProductName = request.ProductName ?? product.ProductName;
            product.Description = request.Description ?? product.Description;
            product.Gender = request.Gender ?? product.Gender;
            product.BaseMaterial = request.BaseMaterial ?? product.BaseMaterial;
            product.Status = request.Status ?? product.Status;
            product.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product with id {ProductId} updated successfully", id);

            return await GetProductResponseByIdAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(
            long id,
            bool softDelete = true,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting product with id {ProductId}. SoftDelete: {SoftDelete}", id, softDelete);

            var product = await _unitOfWork.Products
                .GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            if (softDelete)
            {
                // Soft delete - just mark as inactive
                product.Status = "Inactive";
                product.UpdatedAtUtc = DateTime.UtcNow;
                _unitOfWork.Products.Update(product);
            }
            else
            {
                // Hard delete - remove all related data
                await DeleteProductRelatedDataAsync(id, cancellationToken);
                _unitOfWork.Products.Remove(product);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Product with id {ProductId} deleted successfully", id);
        }

        public async Task<bool> ExistsAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Products
                .AnyAsync(p => p.ProductId == id, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string productCode,
            long? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            if (excludeId.HasValue)
            {
                return await _unitOfWork.Products
                    .AnyAsync(p => p.ProductCode == productCode && p.ProductId != excludeId.Value, cancellationToken);
            }

            return await _unitOfWork.Products
                .AnyAsync(p => p.ProductCode == productCode, cancellationToken);
        }

        public async Task<ProductResponseModel> UpdateStatusAsync(
            long id,
            string status,
            CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            product.Status = status;
            product.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetProductResponseByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<ProductResponseModel>> GetProductsByBrandAsync(
            int brandId,
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            var products = await _unitOfWork.Products
                .Query()
                .Where(p => p.BrandId == brandId && p.Status == "Active")
                .OrderByDescending(p => p.CreatedAtUtc)
                .Take(limit)
                .Select(p => new ProductResponseModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Slug = p.Slug,
                    Description = p.Description,
                    Gender = p.Gender,
                    BaseMaterial = p.BaseMaterial,
                    Status = p.Status,
                    BrandName = p.Brand.BrandName,
                    CategoryName = p.Category.CategoryName,
                    MinPrice = p.ProductVariants
                        .Select(v => (decimal?)v.SalePrice )
                        .Min() ?? 0,
                    MaxPrice = p.ProductVariants
                        .Select(v => (decimal?)v.ListPrice)
                        .Max() ?? 0,
                    ThumbnailUrl = p.ProductImages
                        .OrderBy(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    AverageRating = p.ProductRatings
                        .Select(r => (double?)r.RatingValue)
                        .Average() ?? 0,
                    TotalStock = p.ProductVariants
                        .Sum(v => v.InventoryStock != null
                            ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                            : 0)
                })
                .ToListAsync(cancellationToken);

            return products;
        }

        public async Task<IEnumerable<ProductResponseModel>> GetRelatedProductsAsync(
            long productId,
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.Products
                .GetByIdAsync(productId, cancellationToken);

            if (product == null)
            {
                return Enumerable.Empty<ProductResponseModel>();
            }

            var relatedProducts = await _unitOfWork.Products
                .Query()
                .Where(p => p.ProductId != productId &&
                           p.Status == "Active" &&
                           (p.CategoryId == product.CategoryId || p.BrandId == product.BrandId))
                .OrderByDescending(p => p.CreatedAtUtc)
                .Take(limit)
                .Select(p => new ProductResponseModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Slug = p.Slug,
                    Description = p.Description,
                    Gender = p.Gender,
                    BaseMaterial = p.BaseMaterial,
                    Status = p.Status,
                    BrandName = p.Brand.BrandName,
                    CategoryName = p.Category.CategoryName,
                    MinPrice = p.ProductVariants
                        .Select(v => (decimal?)v.SalePrice )
                        .Min() ?? 0,
                    MaxPrice = p.ProductVariants
                        .Select(v => (decimal?)v.ListPrice)
                        .Max() ?? 0,
                    ThumbnailUrl = p.ProductImages
                        .OrderBy(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    AverageRating = p.ProductRatings
                        .Select(r => (double?)r.RatingValue)
                        .Average() ?? 0,
                    TotalStock = p.ProductVariants
                        .Sum(v => v.InventoryStock != null
                            ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                            : 0)
                })
                .ToListAsync(cancellationToken);

            return relatedProducts;
        }

        // Private helper methods
        private IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductQueryModel filters)
        {
            if (!string.IsNullOrWhiteSpace(filters.Keyword))
            {
                string keyword = filters.Keyword.Trim();
                query = query.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    (p.Description != null && p.Description.Contains(keyword)) ||
                    p.ProductCode.Contains(keyword));
            }

            if (filters.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == filters.BrandId.Value);
            }

            if (filters.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filters.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Gender))
            {
                string gender = filters.Gender.Trim();
                query = query.Where(p => p.Gender == gender);
            }

            if (!string.IsNullOrWhiteSpace(filters.Status))
            {
                string status = filters.Status.Trim();
                query = query.Where(p => p.Status == status);
            }

            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.ProductVariants.Any(v => (v.SalePrice ?? v.ListPrice) >= filters.MinPrice.Value));
            }

            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.ProductVariants.Any(v => v.ListPrice <= filters.MaxPrice.Value));
            }

            return query;
        }

        private ProductDetailResponseModel MapToProductDetailResponse(Product product)
        {
            return new ProductDetailResponseModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                Slug = product.Slug,
                Description = product.Description,
                Gender = product.Gender,
                BaseMaterial = product.BaseMaterial,
                Status = product.Status,
                BrandName = product.Brand?.BrandName ?? string.Empty,
                CategoryName = product.Category?.CategoryName ?? string.Empty,
                AverageRating = product.ProductRatings.Any()
                    ? product.ProductRatings.Average(r => r.RatingValue)
                    : 0,
                TotalStock = product.ProductVariants
                    .Sum(v => v.InventoryStock != null
                        ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                        : 0),
                Variants = product.ProductVariants.Select(v => new ProductVariantResponseModel
                {
                    ProductVariantId = v.ProductVariantId,
                    Sku = v.Sku,
                    ColorCode = v.ColorCode,
                    ColorName = v.ColorName,
                    SizeCode = v.SizeCode,
                    SizeLabel = v.SizeLabel,
                    ListPrice = v.ListPrice,
                    SalePrice = v.SalePrice,
                    Status = v.Status,
                    StockQuantity = v.InventoryStock != null
                        ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                        : 0
                }).ToList(),
                Images = product.ProductImages.Select(i => new ProductImageResponseModel
                {
                    ProductImageId = i.ProductImageId,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary
                }).OrderBy(i => i.SortOrder).ToList(),
                Ratings = product.ProductRatings.Select(r => new ProductRatingResponseModel
                {
                    ProductRatingId = r.ProductRatingId,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User?.UserName ?? r.User?.Email ?? "Anonymous",
                    RatingValue = r.RatingValue,
                    ReviewText = r.ReviewText,
                    CreatedAtUtc = r.CreatedAtUtc
                }).OrderByDescending(r => r.CreatedAtUtc).ToList()
            };
        }

        private async Task<ProductResponseModel> GetProductResponseByIdAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products
                .Query()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with id {id} not found");
            }

            return new ProductResponseModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                Slug = product.Slug,
                Description = product.Description,
                Gender = product.Gender,
                BaseMaterial = product.BaseMaterial,
                Status = product.Status,
                BrandName = product.Brand?.BrandName ?? string.Empty,
                CategoryName = product.Category?.CategoryName ?? string.Empty,
                MinPrice = product.ProductVariants
                    .Select(v => (decimal?)v.SalePrice )
                    .Min() ?? 0,
                MaxPrice = product.ProductVariants
                    .Select(v => (decimal?)v.ListPrice)
                    .Max() ?? 0,
                ThumbnailUrl = product.ProductImages
                    .OrderBy(i => i.IsPrimary)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                AverageRating = 0, // You might want to calculate this if needed
                TotalStock = product.ProductVariants
                    .Sum(v => v.InventoryStock != null
                        ? v.InventoryStock.QuantityOnHand - v.InventoryStock.QuantityReserved
                        : 0)
            };
        }

        private async Task DeleteProductRelatedDataAsync(
            long productId,
            CancellationToken cancellationToken)
        {
            // Get all product variants
            var variants = await _unitOfWork.ProductVariants
                .FindAsync(v => v.ProductId == productId, cancellationToken: cancellationToken);

            if (variants.Any())
            {
                // Delete inventory stock for each variant
                foreach (var variant in variants)
                {
                    if (variant.InventoryStock != null)
                    {
                        _unitOfWork.InventoryStocks.Remove(variant.InventoryStock);
                    }
                }

                // Delete all product variants
                _unitOfWork.ProductVariants.RemoveRange(variants);
            }

            // Delete product images
            var images = await _unitOfWork.ProductImages
                .FindAsync(i => i.ProductId == productId, cancellationToken: cancellationToken);

            if (images.Any())
            {
                _unitOfWork.ProductImages.RemoveRange(images);
            }

            // Delete product ratings
            var ratings = await _unitOfWork.ProductRatings
                .FindAsync(r => r.ProductId == productId, cancellationToken: cancellationToken);

            if (ratings.Any())
            {
                _unitOfWork.ProductRatings.RemoveRange(ratings);
            }
        }

        private string GenerateSlug(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return string.Empty;

            // Convert to lowercase
            string slug = productName.ToLowerInvariant();

            // Replace spaces with hyphens
            slug = slug.Replace(' ', '-');

            // Remove special characters (keep only alphanumeric and hyphens)
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            // Add timestamp suffix for uniqueness if needed (optional)
            if (string.IsNullOrEmpty(slug))
            {
                slug = $"product-{DateTime.UtcNow.Ticks}";
            }

            return slug;
        }
    }
}