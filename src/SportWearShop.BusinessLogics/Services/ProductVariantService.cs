
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.BussinessLogics.Services;

public class ProductVariantService : IProductVariantService{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductVariantService> _logger;

    public ProductVariantService(
        IUnitOfWork unitOfWork, 
        ILogger<ProductVariantService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagingResponseModel<ProductVariantResponseModel>> GetByProductIdAsync(
        long productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving product variants. ProductId={ProductId}, PageNumber={PageNumber}, PageSize={PageSize}",
            productId,
            pageNumber,
            pageSize);

        var productExists = await _unitOfWork.Products.AnyAsync(
            product => product.ProductId == productId
                    && product.Status == ProductStatus.Active,
            cancellationToken);

        if (!productExists)
        {
            _logger.LogWarning(
                "Get variants failed. Product not found. ProductId={ProductId}",
                productId);

            throw new NotFoundException($"Product with ID {productId} was not found.");
        }

        var options = new QueryOptions<ProductVariant>
        {
            Filter = variant => variant.ProductId == productId
                                && variant.Status == ProductVariantStatus.Active,
            SortBy = variant => variant.CreatedAtUtc,
            Ascending = false,
            PageNumber = pageNumber,
            PageSize = pageSize,
            AsNoTracking = true,
            Includes = new List<Expression<Func<ProductVariant, object>>>
            {
                variant => variant.InventoryStock!,
                variant => variant.ProductImages
            }
        };

        var result = await _unitOfWork.ProductVariants.FindWithPagingAsync(
            options,
            selector: variant => new ProductVariantResponseModel
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
            },
            cancellationToken);

        _logger.LogInformation(
            "Retrieved product variants successfully. ProductId={ProductId}, ReturnedItems={ReturnedItems}, TotalCount={TotalCount}",
            productId,
            result.Items.Count,
            result.TotalCount);

        return new PagingResponseModel<ProductVariantResponseModel>(
            result.Items,
            result.TotalCount,
            pageNumber,
            pageSize);
    }

    public async Task<ProductVariantDetailResponseModel?> GetByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving product variant details. ProductVariantId={ProductVariantId}",
            productVariantId);

        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
            predicate: v => v.ProductVariantId == productVariantId
                         && v.Status != ProductVariantStatus.Deleted,
            selector: v => new ProductVariantDetailResponseModel
            {
                ProductVariantId = v.ProductVariantId,
                ProductId = v.ProductId,
                Sku = v.Sku,
                ColorCode = v.ColorCode,
                ColorName = v.ColorName,
                SizeCode = v.SizeCode,
                SizeLabel = v.SizeLabel,
                ListPrice = v.ListPrice,
                SalePrice = v.SalePrice,
                WeightGrams = v.WeightGrams,
                Status = v.Status.ToString(),
                QuantityOnHand = v.InventoryStock == null
                    ? 0
                    : v.InventoryStock.QuantityOnHand,
                QuantityReserved = v.InventoryStock == null
                    ? 0
                    : v.InventoryStock.QuantityReserved,
                Images = v.ProductImages
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
            },
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<ProductVariant, object>>[]
            {
                variant => variant.InventoryStock!,
                variant => variant.ProductImages
            });

        if (variant == null)
        {
            _logger.LogWarning(
                "Get product variant details failed. Variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException($"Product variant with ID {productVariantId} was not found.");
        }

        _logger.LogInformation(
            "Retrieved product variant details successfully. ProductVariantId={ProductVariantId}",
            productVariantId);

        return variant;
    }

    public async Task<List<ProductVariantResponseModel>> CreateManyAsync(
        long productId,
        CreateProductVariantsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Variants == null || request.Variants.Count == 0)
        {
            throw new BadRequestException("At least one product variant is required.");
        }

        _logger.LogInformation(
            "Creating multiple product variants. ProductId={ProductId}, Count={Count}",
            productId,
            request.Variants.Count);

        var productExists = await _unitOfWork.Products.AnyAsync(
            product => product.ProductId == productId
                       && product.Status != ProductStatus.Deleted,
            cancellationToken);

        if (!productExists)
        {
            _logger.LogWarning(
                "Create product variant failed. Product not found. ProductId={ProductId}",
                productId);
            throw new NotFoundException($"Product with ID {productId} was not found.");
        }

        var normalizedRequests = request.Variants.Select(v =>
        {
            var colorCode = v.ColorCode.Trim().ToUpper();
            var sizeCode = v.SizeCode.Trim().ToUpper();

            return new
            {
                Original = v,
                Sku = GenerateSku(productId, colorCode, sizeCode),
                ColorCode = colorCode,
                ColorName = v.ColorName.Trim(),
                SizeCode = sizeCode,
                SizeLabel = v.SizeLabel.Trim()
            };
        }).ToList();
        
        var duplicateSkuInRequest = normalizedRequests
            .GroupBy(v => v.Sku)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateSkuInRequest != null)
        {
            throw new ConflictException(
                $"Duplicate SKU in request: '{duplicateSkuInRequest.Key}'.");
        }

        var duplicateColorSizeInRequest = normalizedRequests
            .GroupBy(v => new { v.ColorCode, v.SizeCode })
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateColorSizeInRequest != null)
        {
            throw new ConflictException(
                $"Duplicate color and size in request: ColorCode='{duplicateColorSizeInRequest.Key.ColorCode}', SizeCode='{duplicateColorSizeInRequest.Key.SizeCode}'.");
        }

        foreach (var item in normalizedRequests)
        {
            if (item.Original.SalePrice.HasValue &&
                item.Original.SalePrice.Value > item.Original.ListPrice)
            {
                throw new BadRequestException(
                    $"Sale price cannot be greater than list price. SKU='{item.Sku}'.");
            }
        }

        var skus = normalizedRequests.Select(v => v.Sku).ToList();

        var skuExists = await _unitOfWork.ProductVariants.AnyAsync(
            variant => skus.Contains(variant.Sku.ToUpper()),
            cancellationToken);

        if (skuExists)
        {
            throw new ConflictException("One or more SKUs already exist.");
        }

        foreach (var item in normalizedRequests)
        {
            var variantCombinationExists = await _unitOfWork.ProductVariants.AnyAsync(
                variant => variant.ProductId == productId
                        && variant.ColorCode == item.ColorCode
                        && variant.SizeCode == item.SizeCode
                        && variant.Status == ProductVariantStatus.Active,
                cancellationToken);

            if (variantCombinationExists)
            {
                throw new ConflictException(
                    $"Product variant color and size already exists. ColorCode='{item.ColorCode}', SizeCode='{item.SizeCode}'.");
            }
        }

        var now = DateTime.UtcNow;

        var variants = normalizedRequests.Select(item => new ProductVariant
        {
            ProductId = productId,
            Sku = item.Sku,
            ColorCode = item.ColorCode,
            ColorName = item.ColorName,
            SizeCode = item.SizeCode,
            SizeLabel = item.SizeLabel,
            ListPrice = item.Original.ListPrice,
            SalePrice = item.Original.SalePrice,
            WeightGrams = item.Original.WeightGrams,
            Status = ProductVariantStatus.Draft,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        }).ToList();

        await _unitOfWork.ProductVariants.AddRangeAsync(variants, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Multiple product variants created successfully. ProductId={ProductId}, Count={Count}",
            productId,
            variants.Count);

        return variants.Select(variant => new ProductVariantResponseModel
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
            QuantityOnHand = 0,
            QuantityReserved = 0,
            Images = new List<ProductImageResponseModel>()
        }).ToList();
    }

    public async Task<ProductVariantResponseModel> UpdateAsync(
        long productVariantId,
        UpdateProductVariantRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Updating product variant. ProductVariantId={ProductVariantId}",
            productVariantId);

        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
            predicate: v => v.ProductVariantId == productVariantId,
            selector: v => v,
            asNoTracking: false,
            cancellationToken: cancellationToken);
        
        if (variant == null)
        {
            _logger.LogWarning(
                "Update product variant failed. Variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);
            throw new NotFoundException($"Product variant with ID {productVariantId} was not found.");
        }

        var normalizedSku = request.Sku.Trim().ToUpper();
        var normalizedColorCode = request.ColorCode.Trim().ToUpper();
        var normalizedColorName = request.ColorName.Trim();
        var normalizedSizeCode = request.SizeCode.Trim().ToUpper();
        var normalizedSizeLabel = request.SizeLabel.Trim();

        var skuExists = await _unitOfWork.ProductVariants.AnyAsync(
            otherVariant => otherVariant.Sku == normalizedSku
                            && otherVariant.ProductVariantId != productVariantId,
            cancellationToken);

        if (skuExists)
        {
            _logger.LogWarning(
                "Update product variant failed. SKU already exists. SKU={Sku}",
                request.Sku);
            throw new ConflictException("SKU already exists.");
        }

        var variantCombinationExists = await _unitOfWork.ProductVariants.AnyAsync(
            otherVariant => otherVariant.ProductId == variant.ProductId
                            && otherVariant.ColorCode == normalizedColorCode
                            && otherVariant.SizeCode == normalizedSizeCode
                            && otherVariant.ProductVariantId != productVariantId
                            && otherVariant.Status == ProductVariantStatus.Active,
            cancellationToken);

        if (variantCombinationExists)
        {
            _logger.LogWarning(
                "Update product variant failed. Variant with same color and size already exists. ProductId={ProductId}, ColorCode={ColorCode}, SizeCode={SizeCode}",
                variant.ProductId,
                request.ColorCode,
                request.SizeCode);
            throw new ConflictException("Product variant color and size already exists.");
        }

        if (request.SalePrice.HasValue && request.SalePrice.Value > request.ListPrice)
        {
            _logger.LogWarning(
                "Update product variant failed. Sale price cannot be greater than list price. ProductId={ProductId}, SalePrice={SalePrice}, ListPrice={ListPrice}",
                variant.ProductId,
                request.SalePrice,
                request.ListPrice);
            throw new BadRequestException("Sale price cannot be greater than list price.");
        }

        variant.Sku = normalizedSku;
        variant.ColorCode = normalizedColorCode;
        variant.ColorName = normalizedColorName;
        variant.SizeCode = normalizedSizeCode;
        variant.SizeLabel = normalizedSizeLabel;
        variant.ListPrice = request.ListPrice;
        variant.SalePrice = request.SalePrice;
        variant.WeightGrams = request.WeightGrams;
        variant.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.ProductVariants.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product variant updated successfully. ProductVariantId={ProductVariantId}, Sku={Sku}",
            variant.ProductVariantId,
            variant.Sku);

        return new ProductVariantResponseModel
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
            QuantityOnHand = 0,
            QuantityReserved = 0
        };
    }

    public async Task DeleteAsync(
        long productVariantId,
        CancellationToken cancellationToken = default
    )
    {
        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
            predicate: v => v.ProductVariantId == productVariantId
                         && v.Status != ProductVariantStatus.Deleted,
            selector: v => v,
            asNoTracking: false,
            cancellationToken: cancellationToken);
        
        if(variant == null)
        {
            _logger.LogWarning(
                "Delete product variant failed. Variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);
            throw new NotFoundException($"Product variant with ID {productVariantId} was not found.");
        }

        variant.Status = ProductVariantStatus.Deleted;
        variant.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.ProductVariants.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }

    public async Task<ProductVariantDetailResponseModel> UpdateSortOrdersAsync(
        long productVariantId,
        UpdateProductImageSortOrdersRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Updating product variant image sort orders. ProductVariantId={ProductVariantId}",
            productVariantId);

        var isProductVariantExist = await _unitOfWork.ProductVariants.AnyAsync(
            predicate: variant => variant.ProductVariantId == productVariantId
                                && variant.Status != ProductVariantStatus.Deleted,
            cancellationToken: cancellationToken);

        if (!isProductVariantExist)
        {
            _logger.LogWarning(
                "Update product variant image sort orders failed. Variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException(
                $"Product variant with ID {productVariantId} was not found.");
        }

        if (request.Images == null || !request.Images.Any())
        {
            throw new BadRequestException("Image sort order request is required.");
        }

        var hasInvalidSortOrder = request.Images.Any(image => image.SortOrder <= 0);

        if (hasInvalidSortOrder)
        {
            throw new BadRequestException("SortOrder must be greater than 0.");
        }

        var hasDuplicateImageIds = request.Images
            .GroupBy(image => image.ProductImageId)
            .Any(group => group.Count() > 1);

        if (hasDuplicateImageIds)
        {
            throw new BadRequestException("Duplicate product image IDs are not allowed.");
        }

        var hasDuplicateSortOrders = request.Images
            .GroupBy(image => image.SortOrder)
            .Any(group => group.Count() > 1);

        if (hasDuplicateSortOrders)
        {
            throw new BadRequestException("Duplicate sort orders are not allowed.");
        }

        var imageIds = request.Images
            .Select(image => image.ProductImageId)
            .ToList();

        var images = await _unitOfWork.ProductImages.FindAsync(
            filter: image => image.ProductVariantId == productVariantId
                            && imageIds.Contains(image.ProductImageId),
            selector: image => image,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (images.Count != imageIds.Count)
        {
            _logger.LogWarning(
                "Update product variant image sort orders failed. One or more images not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException(
                "One or more product images were not found in this product variant.");
        }

        foreach (var image in images)
        {
            var requestImage = request.Images.First(requestImage =>
                    requestImage.ProductImageId == image.ProductImageId);

            image.SortOrder = requestImage.SortOrder;
            // image.IsPrimary = false;
        }

        // var primaryImage = images
        //     .OrderBy(image => image.SortOrder)
        //     .ThenBy(image => image.ProductImageId)
        //     .First();

        // primaryImage.IsPrimary = true;

        _unitOfWork.ProductImages.UpdateRange(images);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated product variant image sort orders successfully. ProductVariantId={ProductVariantId}",
            productVariantId);

        var result = await GetByIdAsync(
            productVariantId,
            cancellationToken);

        return result!;
    }

    public async Task<AdminProductVariantDetailResponseModel> GetAdminByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving admin product variant details. ProductVariantId={ProductVariantId}",
            productVariantId);

        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
            predicate: variant => variant.ProductVariantId == productVariantId,
            selector: variant => new AdminProductVariantDetailResponseModel
            {
                ProductVariantId = variant.ProductVariantId,
                ProductId = variant.ProductId,
                ProductName = variant.Product.ProductName,
                ProductCode = variant.Product.ProductCode,

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
            },
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes:
            [
                variant => variant.Product,
                variant => variant.InventoryStock!,
                variant => variant.ProductImages
            ]);

        if (variant == null)
        {
            _logger.LogWarning(
                "Admin get product variant details failed. Variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException(
                $"Product variant with ID {productVariantId} was not found.");
        }

        _logger.LogInformation(
            "Retrieved admin product variant details successfully. ProductVariantId={ProductVariantId}",
            productVariantId);

        return variant;
    }

    private string GenerateSku(
        long productId,
        string colorCode,
        string sizeCode)
    {
        return $"SPW-{productId}-{colorCode}-{sizeCode}";
    }
}   