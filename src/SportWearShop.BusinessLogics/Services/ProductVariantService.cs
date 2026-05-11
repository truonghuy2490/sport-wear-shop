
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
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

    public async Task<List<ProductVariantResponseModel>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving product variants. ProductId={ProductId}",
            productId);

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

        var variants = await _unitOfWork.ProductVariants.FindAsync(
            filter: variant => variant.ProductId == productId
                               && variant.Status == ProductVariantStatus.Active,
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
            sortBy: variant => variant.CreatedAtUtc,
            ascending: false,
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<ProductVariant, object>>[]
            {
                variant => variant.InventoryStock!,
                variant => variant.ProductImages
            });

        _logger.LogInformation(
            "Retrieved product variants successfully. ProductId={ProductId}, Count={Count}",
            productId,
            variants.Count);

        return variants;
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
                         && v.Status == ProductVariantStatus.Active,
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

    public async Task<ProductVariantResponseModel> CreateAsync(
        long productId,
        CreateProductVariantRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Creating product variant. ProductId={ProductId}, Sku={Sku}",
            productId,
            request.Sku);

        var productExists = await _unitOfWork.Products.AnyAsync(
            product => product.ProductId == productId
                       && product.Status == ProductStatus.Active,
            cancellationToken);

        if (!productExists)
        {
            _logger.LogWarning(
                "Create product variant failed. Product not found. ProductId={ProductId}",
                productId);
            throw new NotFoundException($"Product with ID {productId} was not found.");
        }

        var normalizedSku = request.Sku.Trim().ToUpper();
        var normalizedColorCode = request.ColorCode.Trim().ToUpper();
        var normalizedColorName = request.ColorName.Trim();
        var normalizedSizeCode = request.SizeCode.Trim().ToUpper();
        var normalizedSizeLabel = request.SizeLabel.Trim();

        var skuExists = await _unitOfWork.ProductVariants.AnyAsync(
            predicate: variant => variant.Sku.ToUpper() == normalizedSku,
            cancellationToken: cancellationToken);

        if (skuExists)
        {
            _logger.LogWarning(
                "Create product variant failed. SKU already exists. SKU={Sku}",
                request.Sku);

            throw new ConflictException($"SKU '{request.Sku}' already exists.");
        }

        var variantCombinationExists = await _unitOfWork.ProductVariants.AnyAsync(
            variant => variant.ProductId == productId
                       && variant.ColorCode == normalizedColorCode
                       && variant.SizeCode == normalizedSizeCode
                       && variant.Status == ProductVariantStatus.Active,
            cancellationToken);

        if (variantCombinationExists)
        {
            _logger.LogWarning(
                "Create product variant failed. Variant with same color and size already exists. ProductId={ProductId}, ColorCode={ColorCode}, SizeCode={SizeCode}",
                productId,
                request.ColorCode,
                request.SizeCode);
            throw new ConflictException("Product variant color and size already exists.");
        }

        if (request.SalePrice.HasValue && request.SalePrice.Value > request.ListPrice)
        {
            _logger.LogWarning(
                "Create product variant failed. Sale price cannot be greater than list price. ProductId={ProductId}, SalePrice={SalePrice}, ListPrice={ListPrice}",
                productId,
                request.SalePrice,
                request.ListPrice);
            throw new BadRequestException("Sale price cannot be greater than list price.");
        }

        var now = DateTime.UtcNow;  

        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = normalizedSku,
            ColorCode = normalizedColorCode,
            ColorName = normalizedColorName,
            SizeCode = normalizedSizeCode,
            SizeLabel = normalizedSizeLabel,
            ListPrice = request.ListPrice,
            SalePrice = request.SalePrice,
            WeightGrams = request.WeightGrams,
            Status = ProductVariantStatus.Draft,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    
        await _unitOfWork.ProductVariants.AddAsync(variant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product variant created successfully. ProductVariantId={ProductVariantId}, Sku={Sku}",
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
            QuantityReserved = 0,
            Images = new List<ProductImageResponseModel>()
        };
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
            predicate: v => v.ProductVariantId == productVariantId
                         && v.Status == ProductVariantStatus.Active,
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
                         && v.Status == ProductVariantStatus.Active,
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
                                && variant.Status == ProductVariantStatus.Active,
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

}