using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.BussinessLogics.Services;

public class ProductImageService : IProductImageService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductImageService> _logger;  

    public ProductImageService(IUnitOfWork unitOfWork, ILogger<ProductImageService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<ProductImageResponseModel>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving product images. ProductId={ProductId}",
            productId);

        var isProductExist = await _unitOfWork.Products.AnyAsync(
            product => product.ProductId == productId
                       && product.Status == ProductStatus.Active,
            cancellationToken);

        if(!isProductExist)
        {
            _logger.LogWarning(
                "Get product images failed. Product not found. ProductId={ProductId}",
                productId);

            throw new NotFoundException($"Product with ID {productId} was not found.");
        }

        var images = await _unitOfWork.ProductImages.FindAsync(
            filter: image => image.ProductId == productId,
            selector: image => new ProductImageResponseModel
            {
                ProductImageId = image.ProductImageId,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                SortOrder = image.SortOrder,
                IsPrimary = image.IsPrimary
            },
            asNoTracking: true,
            sortBy: image => image.SortOrder,
            ascending: true,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "Retrieved product images successfully. ProductId={ProductId}, Count={Count}",
            productId,
            images.Count);

        return images;
    }

    public async Task<List<ProductImageResponseModel>> GetByVariantIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving product images. ProductVariantId={ProductVariantId}",
            productVariantId);

        var isVariantExist = await _unitOfWork.ProductVariants.AnyAsync(
            variant => variant.ProductVariantId == productVariantId
                       && variant.Status == ProductVariantStatus.Active,
            cancellationToken);

        if(!isVariantExist)
        {
            _logger.LogWarning(
                "Get product images failed. Product variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException($"Product variant with ID {productVariantId} was not found.");
        }

        var images = await _unitOfWork.ProductImages.FindAsync(
            filter: image => image.ProductVariantId == productVariantId,
            selector: image => new ProductImageResponseModel
            {
                ProductImageId = image.ProductImageId,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                SortOrder = image.SortOrder,
                IsPrimary = image.IsPrimary
            },
            asNoTracking: true,
            sortBy: image => image.SortOrder,
            ascending: true,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "Retrieved product images successfully. ProductVariantId={ProductVariantId}, Count={Count}",
            productVariantId,
            images.Count);

        return images;
    }


    public async Task<ProductImageResponseModel> CreateAsync(
        CreateProductImageRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Creating product image. ProductId={ProductId}, ProductVariantId={ProductVariantId}",
            request.ProductId,
            request.ProductVariantId);

        var isProductExist = await _unitOfWork.Products.AnyAsync(
            product => product.ProductId == request.ProductId
                       && product.Status == ProductStatus.Active,
            cancellationToken);
        if (!isProductExist)
        {
            _logger.LogWarning(
                "Create product image failed. Product not found. ProductId={ProductId}",
                request.ProductId);

            throw new NotFoundException($"Product with ID {request.ProductId} was not found.");
        }

        if (request.ProductVariantId.HasValue)
        {
            var isVariantExist = await _unitOfWork.ProductVariants.AnyAsync(
                variant => variant.ProductVariantId == request.ProductVariantId.Value
                           && variant.ProductId == request.ProductId
                           && variant.Status == ProductVariantStatus.Active,
                cancellationToken);

            if (!isVariantExist)
            {
                _logger.LogWarning(
                    "Create product image failed. Variant not found or does not belong to product. ProductId={ProductId}, ProductVariantId={ProductVariantId}",
                    request.ProductId,
                    request.ProductVariantId);

                throw new NotFoundException(
                    $"Product variant with ID {request.ProductVariantId} was not found in product {request.ProductId}.");
            }
        }
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            throw new BadRequestException("Image URL is required.");
        }

        var imageUrl = request.ImageUrl.Trim();
        var altText = request.AltText?.Trim();

        if (request.IsPrimary)
        {
            await ClearPrimaryImagesAsync(
                request.ProductId,
                request.ProductVariantId,
                cancellationToken);
        }

        var image = new ProductImage
        {
            ProductId = request.ProductId,
            ProductVariantId = request.ProductVariantId,
            ImageUrl = imageUrl,
            AltText = altText,
            SortOrder = request.SortOrder,
            IsPrimary = request.IsPrimary,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.ProductImages.AddAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product image created successfully. ProductImageId={ProductImageId}, ProductId={ProductId}, ProductVariantId={ProductVariantId}",
            image.ProductImageId,
            image.ProductId,
            image.ProductVariantId);

        return new ProductImageResponseModel
        {
            ProductImageId = image.ProductImageId,
            ImageUrl = image.ImageUrl,
            AltText = image.AltText,
            SortOrder = image.SortOrder,
            IsPrimary = image.IsPrimary
        };
    }

    public async Task<ProductImageResponseModel> UpdateAsync(
        long productImageId,
        UpdateProductImageRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Updating product image. ProductImageId={ProductImageId}",
            productImageId);

        var image = await _unitOfWork.ProductImages.FirstOrDefaultAsync(
            predicate: image => image.ProductImageId == productImageId,
            selector: image => image,
            asNoTracking: false,
            cancellationToken: cancellationToken);
        if(image == null)
        {
            _logger.LogWarning(
                "Update product image failed. ProductImage not found. ProductImageId={ProductImageId}",
                productImageId);

            throw new NotFoundException(
                $"Product image with ID {productImageId} was not found.");
        }
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            throw new BadRequestException("Image URL is required.");
        }

        if (request.IsPrimary && !image.IsPrimary)
        {
            await ClearPrimaryImagesAsync(
                image.ProductId,
                image.ProductVariantId,
                cancellationToken);
        }

        image.ImageUrl = request.ImageUrl.Trim();
        image.AltText = request.AltText?.Trim();
        image.SortOrder = request.SortOrder;
        image.IsPrimary = request.IsPrimary;

        _unitOfWork.ProductImages.Update(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product image updated successfully. ProductImageId={ProductImageId}",
            image.ProductImageId);

        return new ProductImageResponseModel
        {
            ProductImageId = image.ProductImageId,
            ImageUrl = image.ImageUrl,
            AltText = image.AltText,
            SortOrder = image.SortOrder,
            IsPrimary = image.IsPrimary
        };
    }

    public async Task SetPrimaryAsync(
        long productImageId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Setting product image as primary. ProductImageId={ProductImageId}",
            productImageId);

        var image = await _unitOfWork.ProductImages.FirstOrDefaultAsync(
            predicate: image => image.ProductImageId == productImageId,
            selector: image => image,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (image == null)
        {
            _logger.LogWarning(
                "Set primary image failed. ProductImage not found. ProductImageId={ProductImageId}",
                productImageId);

            throw new NotFoundException(
                $"Product image with ID {productImageId} was not found.");
        }

        await ClearPrimaryImagesAsync(
            image.ProductId,
            image.ProductVariantId,
            cancellationToken);

        image.IsPrimary = true;

        _unitOfWork.ProductImages.Update(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product image set as primary successfully. ProductImageId={ProductImageId}",
            productImageId);
    }

    public async Task DeleteAsync(
        long productImageId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Deleting product image. ProductImageId={ProductImageId}",
            productImageId);

        var image = await _unitOfWork.ProductImages.FirstOrDefaultAsync(
            predicate: image => image.ProductImageId == productImageId,
            selector: image => image,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (image == null)
        {
            _logger.LogWarning(
                "Delete product image failed. ProductImage not found. ProductImageId={ProductImageId}",
                productImageId);

            throw new NotFoundException(
                $"Product image with ID {productImageId} was not found.");
        }

        _unitOfWork.ProductImages.Remove(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product image deleted successfully. ProductImageId={ProductImageId}",
            productImageId);
    }

    private async Task ClearPrimaryImagesAsync(
        long productId,
        long? productVariantId,
        CancellationToken cancellationToken = default)
    {
        var images = await _unitOfWork.ProductImages.FindAsync(
            filter: image => image.ProductId == productId
                             && image.ProductVariantId == productVariantId
                             && image.IsPrimary,
            selector: image => image,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        foreach (var image in images)
        {
            image.IsPrimary = false;
        }

        if (images.Any())
        {
            _unitOfWork.ProductImages.UpdateRange(images);
        }
    }
}   