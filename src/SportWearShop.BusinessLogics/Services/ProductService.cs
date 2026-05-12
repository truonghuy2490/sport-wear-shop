using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Helpers;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;
using System.Linq.Expressions;
using LinqKit;

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

        public async Task<PagingResponseModel<ProductResponseModel>> GetAllAsync(
            ProductQueryRequestModel request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(
                "Retrieving products. PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                request.PageNumber,
                request.PageSize,
                request.SearchTerm);
            var filter = PredicateBuilder.New<Product>(true);
            filter = filter.And(
                product => product.Status == (request.Status ?? ProductStatus.Active)
            );

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var keyword = request.SearchTerm.Trim();

                filter = filter.And(product =>
                    product.ProductName.Contains(keyword) ||
                    product.ProductCode.Contains(keyword) ||
                    product.Slug.Contains(keyword));
            }

            if (request.BrandIds is { Count: > 0 })
            {
                filter = filter.And(product =>
                    request.BrandIds.Contains(product.BrandId));
            }

            if (request.CategoryIds is { Count: > 0 })
            {
                filter = filter.And(product =>
                    request.CategoryIds.Contains(product.CategoryId) ||
                    request.CategoryIds.Contains(product.Category.ParentCategoryId ?? 0) ||
                    request.CategoryIds.Contains(product.Category.ParentCategory!.ParentCategoryId ?? 0));
            }

            if (request.Gender.HasValue)
            {
                filter = filter.And(product =>
                    product.Gender == request.Gender.Value);
            }

            if (request.MinPrice.HasValue)
            {
                filter = filter.And(product =>
                    product.ProductVariants.Any(variant =>
                        variant.Status == ProductVariantStatus.Active &&
                        variant.ListPrice >= request.MinPrice.Value));
            }

            if (request.MaxPrice.HasValue)
            {
                filter = filter.And(product =>
                    product.ProductVariants.Any(variant =>
                        variant.Status == ProductVariantStatus.Active &&
                        variant.ListPrice <= request.MaxPrice.Value));
            }

            if (request.IsOnSale == true)
            {
                filter = filter.And(product =>
                    product.ProductVariants.Any(variant =>
                        variant.Status == ProductVariantStatus.Active &&
                        variant.SalePrice != null &&
                        variant.SalePrice < variant.ListPrice));
            }

            if (request.IsNewRelease == true)
            {
                var newReleaseFromDate = DateTime.UtcNow.AddDays(-30);

                filter = filter.And(product =>
                    product.CreatedAtUtc >= newReleaseFromDate);
            }

            if (request.CreatedFromUtc.HasValue)
            {
                filter = filter.And(product =>
                    product.CreatedAtUtc >= request.CreatedFromUtc.Value);
            }

            if (request.CreatedToUtc.HasValue)
            {
                filter = filter.And(product =>
                    product.CreatedAtUtc <= request.CreatedToUtc.Value);
            }

            var options = new QueryOptions<Product>
            {
                Filter = filter,
                SortBy = GetSortExpression(request.SortBy),
                Ascending = request.IsAscending,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                AsNoTracking = true,
                Includes = new List<Expression<Func<Product, object>>>
                {
                    product => product.Brand,
                    product => product.Category,
                    product => product.ProductImages,
                    product => product.ProductVariants
                }
            };

            var result = await _unitOfWork.Products.FindWithPagingAsync(
                options,
                selector: product => new ProductResponseModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Slug = product.Slug,

                    BrandName = product.Brand.BrandName,
                    CategoryName = product.Category.CategoryName,

                    Gender = product.Gender.ToString(),
                    Status = product.Status.ToString(),

                    ThumbnailUrl = product.ProductImages
                        .Where(image => image.IsPrimary)
                        .Select(image => image.ImageUrl)
                        .FirstOrDefault(),

                    MinPrice = product.ProductVariants
                        .Where(variant => variant.Status == ProductVariantStatus.Active)
                        .Select(variant => variant.ListPrice)
                        .DefaultIfEmpty()
                        .Min(),

                    MinSalePrice = product.ProductVariants
                        .Where(variant =>
                            variant.Status == ProductVariantStatus.Active &&
                            variant.SalePrice != null)
                        .Select(variant => variant.SalePrice)
                        .DefaultIfEmpty()
                        .Min(),

                    TotalVariants = product.ProductVariants.Count(
                        variant => variant.Status == ProductVariantStatus.Active)
                },
                cancellationToken);

            _logger.LogInformation(
                "Returning products. PageNumber={PageNumber}, PageSize={PageSize}, ReturnedItems={ReturnedItems}, TotalCount={TotalCount}",
                request.PageNumber,
                request.PageSize,
                result.Items,
                result.TotalCount);

            return new PagingResponseModel<ProductResponseModel>(
                result.Items,
                result.TotalCount,
                request.PageNumber,
                request.PageSize
            );
        }

        public async Task<ProductDetailResponseModel> GetDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
               "Retrieving product details. ProductId={ProductId}",
               productId);

            var product = await _unitOfWork.Products.FirstOrDefaultAsync(
                predicate: product => product.ProductId == productId
                                      && product.Status == ProductStatus.Active,
                selector: product => new ProductDetailResponseModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Slug = product.Slug,
                    Description = product.Description,
                    Gender = product.Gender.ToString(),
                    BaseMaterial = product.BaseMaterial,
                    BrandName = product.Brand.BrandName,
                    CategoryName = product.Category.CategoryName,
                    Status = product.Status.ToString(),
                    CreatedAtUtc = product.CreatedAtUtc,
                    UpdatedAtUtc = product.UpdatedAtUtc,

                    Images = product.ProductImages
                        .OrderBy(image => image.SortOrder)
                        .Select(image => new ProductImageResponseModel
                        {
                            ProductImageId = image.ProductImageId,
                            ImageUrl = image.ImageUrl,
                            AltText = image.AltText,
                            SortOrder = image.SortOrder,
                            IsPrimary = image.IsPrimary
                        })
                        .ToList(),

                    Variants = product.ProductVariants
                        .Where(variant => variant.Status == ProductVariantStatus.Active)
                        .Select(variant => new ProductVariantResponseModel
                        {
                            ProductVariantId = variant.ProductVariantId,
                            Sku = variant.Sku,
                            ColorCode = variant.ColorCode,
                            ColorName = variant.ColorName,
                            SizeCode = variant.SizeCode,
                            SizeLabel = variant.SizeLabel,
                            ListPrice = variant.ListPrice,
                            SalePrice = variant.SalePrice,
                            WeightGrams = variant.WeightGrams,
                            Status = variant.Status.ToString(),
                            QuantityOnHand = variant.InventoryStock == null
                                ? 0
                                : variant.InventoryStock.QuantityOnHand,
                            QuantityReserved = variant.InventoryStock == null
                                ? 0
                                : variant.InventoryStock.QuantityReserved,
                            Images = variant.ProductImages
                                .OrderBy(image => image.SortOrder)
                                .Select(image => new ProductImageResponseModel
                                {
                                    ProductImageId = image.ProductImageId,
                                    ImageUrl = image.ImageUrl,
                                    AltText = image.AltText,
                                    SortOrder = image.SortOrder,
                                    IsPrimary = image.IsPrimary
                                })
                                .ToList()
                        })
                        .ToList()
                },
                asNoTracking: true,
                cancellationToken: cancellationToken,
                includes: new Expression<Func<Product, object>>[]
                {
                    product => product.Brand,
                    product => product.Category,
                    product => product.ProductImages,
                    product => product.ProductVariants
                }
            );

            if (product == null)
            {
                _logger.LogWarning(
                    "Product details retrieval failed. Product not found. ProductId={ProductId}",
                    productId);

                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            _logger.LogInformation(
                "Retrieved product details successfully. ProductId={ProductId}, VariantCount={VariantCount}, ImageCount={ImageCount}",
                product.ProductId,
                product.Variants.Count,
                product.Images.Count);

            return product;
        }

        public async Task<AdminProductDetailResponseModel> GetAdminDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Retrieving admin product details. ProductId={ProductId}",
                productId);

            var product = await _unitOfWork.Products.FirstOrDefaultAsync(
                predicate: product => product.ProductId == productId,
                selector: product => new AdminProductDetailResponseModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Slug = product.Slug,
                    Description = product.Description,
                    Gender = product.Gender.ToString(),
                    BaseMaterial = product.BaseMaterial,

                    BrandId = product.BrandId,
                    BrandName = product.Brand.BrandName,

                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.CategoryName,

                    Status = product.Status.ToString(),
                    CreatedAtUtc = product.CreatedAtUtc,
                    UpdatedAtUtc = product.UpdatedAtUtc,

                    Images = product.ProductImages
                        .OrderBy(image => image.SortOrder)
                        .Select(image => new AdminProductImageResponseModel
                        {
                            ProductImageId = image.ProductImageId,
                            ProductVariantId = image.ProductVariantId,
                            ImageUrl = image.ImageUrl,
                            AltText = image.AltText,
                            SortOrder = image.SortOrder,
                            IsPrimary = image.IsPrimary
                        })
                        .ToList(),

                    Variants = product.ProductVariants
                        .OrderBy(variant => variant.ColorName)
                        .ThenBy(variant => variant.SizeCode)
                        .Select(variant => new AdminProductVariantResponseModel
                        {
                            ProductVariantId = variant.ProductVariantId,
                            ProductId = variant.ProductId,
                            Sku = variant.Sku,
                            ColorCode = variant.ColorCode,
                            ColorName = variant.ColorName,
                            SizeCode = variant.SizeCode,
                            SizeLabel = variant.SizeLabel,
                            ListPrice = variant.ListPrice,
                            SalePrice = variant.SalePrice,
                            WeightGrams = variant.WeightGrams,
                            Status = variant.Status.ToString(),

                            QuantityOnHand = variant.InventoryStock == null
                                ? 0
                                : variant.InventoryStock.QuantityOnHand,

                            QuantityReserved = variant.InventoryStock == null
                                ? 0
                                : variant.InventoryStock.QuantityReserved,

                            AvailableQuantity = variant.InventoryStock == null
                                ? 0
                                : variant.InventoryStock.QuantityOnHand - variant.InventoryStock.QuantityReserved,

                            Images = variant.ProductImages
                                .OrderBy(image => image.SortOrder)
                                .Select(image => new AdminProductImageResponseModel
                                {
                                    ProductImageId = image.ProductImageId,
                                    ProductVariantId = image.ProductVariantId,
                                    ImageUrl = image.ImageUrl,
                                    AltText = image.AltText,
                                    SortOrder = image.SortOrder,
                                    IsPrimary = image.IsPrimary
                                })
                                .ToList()
                        })
                        .ToList()
                },
                asNoTracking: true,
                cancellationToken: cancellationToken,
                includes:
                [
                    product => product.Brand,
                    product => product.Category,
                    product => product.ProductImages,
                    product => product.ProductVariants
                ]);

            if (product == null)
            {
                _logger.LogWarning(
                    "Admin product details retrieval failed. Product not found. ProductId={ProductId}",
                    productId);

                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            _logger.LogInformation(
                "Retrieved admin product details successfully. ProductId={ProductId}, VariantCount={VariantCount}, ImageCount={ImageCount}",
                product.ProductId,
                product.Variants.Count,
                product.Images.Count);

            return product;
        }

        public async Task<ProductResponseModel> CreateAsync(
            CreateProductRequestModel request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(
                "Creating product. ProductName={ProductName}, BrandId={BrandId}, CategoryId={CategoryId}",
                request.ProductName,
                request.BrandId,
                request.CategoryId);

            var normalizedProductName = request.ProductName.Trim();

            var isBrandExist = await _unitOfWork.Brands.AnyAsync(
                brand => brand.BrandId == request.BrandId && brand.IsActive,
                cancellationToken);

            if (!isBrandExist)
            {
                throw new NotFoundException($"Brand with ID {request.BrandId} was not found.");
            }

            var isCategoryExist = await _unitOfWork.Categories.AnyAsync(
                category => category.CategoryId == request.CategoryId && category.IsActive,
                cancellationToken);

            if (!isCategoryExist)
            {
                throw new NotFoundException($"Category with ID {request.CategoryId} was not found.");
            }

            var normalizedSlug = string.IsNullOrWhiteSpace(request.Slug)
                ? request.ProductName.Trim().ToLower().Replace(" ", "-")
                : request.Slug.Trim().ToLower().Replace(" ", "-");

            var originalSlug = normalizedSlug;
            var slugCounter = 1;

            while (await _unitOfWork.Products.AnyAsync(
                    product => product.Slug == normalizedSlug,
                    cancellationToken))
            {
                slugCounter++;
                normalizedSlug = $"{originalSlug}-{slugCounter}";
            }

            var gender = request.Gender;
            var now = DateTime.UtcNow;

            var product = new Product
            {
                BrandId = request.BrandId,
                CategoryId = request.CategoryId,
                ProductName = normalizedProductName,
                ProductCode = string.Empty,
                Slug = normalizedSlug,
                Description = request.Description?.Trim(),
                Gender = gender,
                BaseMaterial = request.BaseMaterial?.Trim(),
                Status = ProductStatus.Draft,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            product.ProductCode = $"PRD-{product.ProductId:D6}";
            product.UpdatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product created successfully. ProductId={ProductId}, ProductCode={ProductCode}, Status={Status}",
                product.ProductId,
                product.ProductCode,
                product.Status);

            return new ProductResponseModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                Slug = product.Slug,
                Gender = product.Gender.ToString(),
                Status = product.Status.ToString(),
                ThumbnailUrl = null,
                MinPrice = 0,
                MinSalePrice = null,
                TotalVariants = 0
            };
        }

        public async Task<ProductResponseModel> UpdateAsync(
            long productId,
            UpdateProductRequestModel request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(
                "Updating product. ProductId={ProductId}, ProductName={ProductName}",
                productId,
                request.ProductName);

            var product = await _unitOfWork.Products.FirstOrDefaultAsync(
                predicate: product => product.ProductId == productId
                                    && product.Status != ProductStatus.Deleted,
                selector: product => product,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            if (product == null)
            {
                _logger.LogWarning(
                    "Update product failed. Product not found. ProductId={ProductId}",
                    productId);

                throw new NotFoundException($"Product with ID {productId} was not found.");
            }

            if (request.BrandId.HasValue)
            {
                var brandExists = await _unitOfWork.Brands.AnyAsync(
                    brand => brand.BrandId == request.BrandId.Value && brand.IsActive,
                    cancellationToken);

                if (!brandExists)
                {
                    throw new NotFoundException($"Brand with ID {request.BrandId.Value} was not found.");
                }

                product.BrandId = request.BrandId.Value;
            }

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _unitOfWork.Categories.AnyAsync(
                    category => category.CategoryId == request.CategoryId.Value && category.IsActive,
                    cancellationToken);

                if (!categoryExists)
                {
                    throw new NotFoundException($"Category with ID {request.CategoryId.Value} was not found.");
                }

                product.CategoryId = request.CategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                product.ProductName = request.ProductName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Slug))
            {
                var normalizedSlug = request.Slug.Trim().ToLower();

                var slugExists = await _unitOfWork.Products.AnyAsync(
                    product => product.Slug == normalizedSlug
                            && product.ProductId != productId,
                    cancellationToken);

                if (slugExists)
                {
                    throw new ConflictException("Product slug already exists.");
                }

                product.Slug = normalizedSlug;
            }

            if (request.Description != null)
            {
                product.Description = request.Description.Trim();
            }

            if (request.Gender.HasValue)
            {
                product.Gender = request.Gender.Value;
            }

            if (request.Status.HasValue)
            {
                product.Status = request.Status.Value;
            }

            if (request.BaseMaterial != null)
            {
                product.BaseMaterial = request.BaseMaterial.Trim();
            }

            product.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product updated successfully. ProductId={ProductId}, ProductCode={ProductCode}",
                product.ProductId,
                product.ProductCode);

            return new ProductResponseModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                Slug = product.Slug,
                Gender = product.Gender.ToString(),
                Status = product.Status.ToString()
            };
        }        
        
        public async Task DeleteAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Deleting product. ProductId={ProductId}",
                productId);

            var product = await _unitOfWork.Products.FirstOrDefaultAsync(
                predicate: product => product.ProductId == productId
                                    && product.Status != ProductStatus.Deleted,
                selector: product => product,
                asNoTracking: false,
                cancellationToken: cancellationToken,
                includes: product => product.ProductVariants);

            if (product == null)
            {
                _logger.LogWarning(
                    "Delete product failed. Product not found. ProductId={ProductId}",
                    productId);

                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            var now = DateTime.UtcNow;

            product.Status = ProductStatus.Deleted;
            product.UpdatedAtUtc = now;

            foreach (var variant in product.ProductVariants)
            {
                variant.Status = ProductVariantStatus.Deleted;
                variant.UpdatedAtUtc = now;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product deleted successfully. ProductId={ProductId}, DeletedVariants={DeletedVariants}",
                productId,
                product.ProductVariants.Count);
        }
        
        private static Expression<Func<Product, object>> GetSortExpression(ProductSortBy sortBy)
        {
            return sortBy switch
            {
                ProductSortBy.ProductName => p => p.ProductName,
                ProductSortBy.ProductCode => p => p.ProductCode,
                ProductSortBy.BrandName => p => p.Brand.BrandName,
                ProductSortBy.CategoryName => p => p.Category.CategoryName,
                ProductSortBy.UpdatedAtUtc => p => p.UpdatedAtUtc,
                _ => p => p.CreatedAtUtc
            };
        }
    }
    
}