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

        public async Task<PagingResponseModel<ProductResponseModel>> GetAllAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Retrieving products. Requested PageNumber={PageNumber}, PageSize={PageSize}",
                pageNumber,
                pageSize);

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalCount = await _unitOfWork.Products.CountAsync(
                product => product.Status == ProductStatus.Active,
                cancellationToken);


            var products = await _unitOfWork.Products.FindAsync(
                filter: product => product.Status == ProductStatus.Active,
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
                        .OrderBy(image => image.SortOrder)
                        .Select(image => image.ImageUrl)
                        .FirstOrDefault(),
                    MinPrice = product.ProductVariants
                        .Where(variant => variant.Status == ProductVariantStatus.Active)
                        .Select(variant => variant.ListPrice)
                        .DefaultIfEmpty()
                        .Min(),
                    MinSalePrice = product.ProductVariants
                        .Where(variant => variant.Status == ProductVariantStatus.Active
                                          && variant.SalePrice != null)
                        .Select(variant => variant.SalePrice)
                        .DefaultIfEmpty()
                        .Min(),
                    TotalVariants = product.ProductVariants.Count(
                        variant => variant.Status == ProductVariantStatus.Active)
                },
                sortBy: product => product.CreatedAtUtc,
                ascending: false,
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
            // TODO: 
            // best practice : do pagination in database query, not in memory
            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation(
                "Returning paged products. PageNumber={PageNumber}, PageSize={PageSize}, ReturnedItems={ReturnedItems}",
                pageNumber,
                pageSize,
                pagedProducts.Count);

            return new PagingResponseModel<ProductResponseModel>(
                pagedProducts,
                totalCount,
                pageNumber,
                pageSize);
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

        public async Task<ProductResponseModel> CreateAsync(
            CreateProductRequestModel request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(
                "Creating product. ProductCode={ProductCode}, ProductName={ProductName}, BrandId={BrandId}, CategoryId={CategoryId}",
                request.ProductCode,
                request.ProductName,
                request.BrandId,
                request.CategoryId);

            var normalizedProductCode = request.ProductCode.Trim().ToUpper();
            var normalizedProductName = request.ProductName.Trim();
            var normalizedSlug = request.Slug.Trim().ToLower();

            var isBrandExist = await _unitOfWork.Brands.AnyAsync(
                brand => brand.BrandId == request.BrandId && brand.IsActive,
                cancellationToken);

            if (!isBrandExist)
            {
                _logger.LogWarning(
                    "Create product failed. Brand not found. BrandId={BrandId}",
                    request.BrandId);

                throw new NotFoundException($"Brand with ID {request.BrandId} was not found.");
            }

            var categoryExists = await _unitOfWork.Categories.AnyAsync(
                category => category.CategoryId == request.CategoryId && category.IsActive,
                cancellationToken);

            if (!categoryExists)
            {
                _logger.LogWarning(
                    "Create product failed. Category not found. CategoryId={CategoryId}",
                    request.CategoryId);

                throw new NotFoundException($"Category with ID {request.CategoryId} was not found.");
            }

            var productCodeExists = await _unitOfWork.Products.AnyAsync(
                product => product.ProductCode == normalizedProductCode,
                cancellationToken);

            if (productCodeExists)
            {
                _logger.LogWarning(
                    "Create product failed. Duplicate ProductCode={ProductCode}",
                    normalizedProductCode);

                throw new ConflictException("Product code already exists.");
            }

            var slugExists = await _unitOfWork.Products.AnyAsync(
                product => product.Slug == normalizedSlug,
                cancellationToken);

            if (slugExists)
            {
                _logger.LogWarning(
                    "Create product failed. Duplicate Slug={Slug}",
                    normalizedSlug);

                throw new ConflictException("Product slug already exists.");
            }

            var gender = EnumHelper.ParseEnum<ProductGender>(request.Gender);

            var now = DateTime.UtcNow;

            var product = new Product
            {
                BrandId = request.BrandId,
                CategoryId = request.CategoryId,
                ProductName = normalizedProductName,
                ProductCode = normalizedProductCode,
                Slug = normalizedSlug,
                Description = request.Description,
                Gender = gender,
                BaseMaterial = request.BaseMaterial,
                Status = ProductStatus.Draft,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
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
                "Updating product. ProductId={ProductId}, ProductCode={ProductCode}, ProductName={ProductName}",
                productId,
                request.ProductCode,
                request.ProductName);

            var normalizedProductCode = request.ProductCode.Trim().ToUpper();
            var normalizedProductName = request.ProductName.Trim();
            var normalizedSlug = request.Slug.Trim().ToLower();

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

            var brandExists = await _unitOfWork.Brands.AnyAsync(
                brand => brand.BrandId == request.BrandId && brand.IsActive,
                cancellationToken);

            if (!brandExists)
            {
                _logger.LogWarning(
                    "Update product failed. Brand not found. BrandId={BrandId}, ProductId={ProductId}",
                    request.BrandId,
                    productId);

                throw new NotFoundException($"Brand with ID {request.BrandId} was not found.");
            }

            var categoryExists = await _unitOfWork.Categories.AnyAsync(
                category => category.CategoryId == request.CategoryId && category.IsActive,
                cancellationToken);

            if (!categoryExists)
            {
                _logger.LogWarning(
                    "Update product failed. Category not found. CategoryId={CategoryId}, ProductId={ProductId}",
                    request.CategoryId,
                    productId);

                throw new NotFoundException($"Category with ID {request.CategoryId} was not found.");
            }

            var productCodeExists = await _unitOfWork.Products.AnyAsync(
                product => product.ProductCode == normalizedProductCode
                           && product.ProductId != productId,
                cancellationToken);

            if (productCodeExists)
            {
                _logger.LogWarning(
                    "Update product failed. Duplicate ProductCode={ProductCode}, ProductId={ProductId}",
                    normalizedProductCode,
                    productId);

                throw new ConflictException("Product code already exists.");
            }

            var slugExists = await _unitOfWork.Products.AnyAsync(
                product => product.Slug == normalizedSlug
                           && product.ProductId != productId,
                cancellationToken);

            if (slugExists)
            {
                _logger.LogWarning(
                    "Update product failed. Duplicate Slug={Slug}, ProductId={ProductId}",
                    normalizedSlug,
                    productId);

                throw new ConflictException("Product slug already exists.");
            }

            if (string.IsNullOrWhiteSpace(request.Gender))
            {
                _logger.LogWarning(
                    "Update product failed. Gender is required. ProductId={ProductId}",
                    productId);

                throw new BadRequestException("Gender is required.");
            }

            var gender = EnumHelper.ParseEnum<ProductGender>(request.Gender);

            product.BrandId = request.BrandId.Value;
            product.CategoryId = request.CategoryId.Value;
            product.ProductName = normalizedProductName;
            product.ProductCode = normalizedProductCode;
            product.Slug = normalizedSlug;
            product.Description = request.Description;
            product.Gender = gender;
            product.BaseMaterial = request.BaseMaterial;
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
                cancellationToken: cancellationToken);

            if (product == null)
            {
                _logger.LogWarning(
                    "Delete product failed. Product not found. ProductId={ProductId}",
                    productId);

                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            product.Status = ProductStatus.Deleted;
            product.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product deleted successfully (soft delete). ProductId={ProductId}",
                productId);
        }
    }
}