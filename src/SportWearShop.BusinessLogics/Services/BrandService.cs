using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.ThirdPartyServices;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;
public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BrandService> _logger;
    private readonly ICloudinaryService _cloudinaryService;

    public BrandService(
        IUnitOfWork unitOfWork,
        ILogger<BrandService> logger,
        ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<PagingResponseModel<BrandResponseModel>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving active brands. PageNumber={PageNumber}, PageSize={PageSize}",
            pageNumber,
            pageSize);

        var options = new QueryOptions<Brand>
        {
            Filter = brand => brand.IsActive,
            SortBy = brand => brand.BrandName,
            Ascending = true,
            PageNumber = pageNumber,
            PageSize = pageSize,
            AsNoTracking = true
        };

        var result = await _unitOfWork.Brands.FindWithPagingAsync(
            options,
            selector: brand => new BrandResponseModel
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                BrandCode = brand.BrandCode,
                BrandImage = brand.BrandImage,
                IsActive = brand.IsActive
            },
            cancellationToken);

        _logger.LogInformation(
            "Retrieved active brands. ReturnedItems={ReturnedItems}, TotalCount={TotalCount}",
            result.Items.Count,
            result.TotalCount);

        return new PagingResponseModel<BrandResponseModel>(
            result.Items,
            result.TotalCount,
            pageNumber,
            pageSize);
    }

    public async Task<BrandDetailResponseModel?> GetByIdAsync(
        int brandId,
        CancellationToken cancellationToken = default)
    {

        var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(
            predicate: brand => brand.BrandId == brandId && brand.IsActive,
            selector: brand => new BrandDetailResponseModel
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                BrandCode = brand.BrandCode,
                BrandImage = brand.BrandImage,
                IsActive = brand.IsActive,
                CreatedAtUtc = brand.CreatedAtUtc,
                UpdatedAtUtc = brand.UpdatedAtUtc,
                ProductCount = brand.Products.Count(product => product.Status == ProductStatus.Active)
            },
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Brand, object>>[]
            {
                brand => brand.Products
            });

        if (brand == null)
        {
            _logger.LogWarning("Brand with ID {BrandId} not found.", brandId);
            throw new NotFoundException($"Brand with ID {brandId} not found.");
        }

        _logger.LogInformation("Retrieved brand with ID {BrandId}.", brandId);
        return brand;
    }

    public async Task<BrandResponseModel> CreateAsync(
        CreateBrandRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation(
            "Creating new brand with Code={BrandCode}, Name={BrandName}",
            request.BrandCode,
            request.BrandName);

        var normalizedBrandCode = request.BrandCode.Trim().ToUpper();
        var normalizedBrandName = request.BrandName.Trim();

        var isBrandCodeExists = await _unitOfWork.Brands.AnyAsync(
            predicate: brand => brand.BrandCode == normalizedBrandCode,
            cancellationToken: cancellationToken);

        if (isBrandCodeExists)
        {
            _logger.LogWarning(
                "Create brand failed. Duplicate BrandCode={BrandCode}",
                normalizedBrandCode);

            throw new ConflictException("Brand code already exists.");
        }

        var isBrandNameExist = await _unitOfWork.Brands.AnyAsync(
                predicate: brand => brand.BrandName == normalizedBrandName && brand.IsActive,
                cancellationToken: cancellationToken);

        if (isBrandNameExist)
        {
            _logger.LogWarning(
                "Create brand failed. Duplicate BrandName={BrandName}",
                normalizedBrandName);

            throw new ConflictException("Brand name already exists.");
        }

        string? brandImageUrl = null;

        if (request.BrandImageFile is { Length: > 0 })
        {
            brandImageUrl = await _cloudinaryService.UploadFileAsync(
                request.BrandImageFile,
                folder: "sport-wear-shop/brands",
                cancellationToken: cancellationToken);
        }

        var brand = new Brand
        {
            BrandName = normalizedBrandName,
            BrandCode = normalizedBrandCode,
            BrandImage = brandImageUrl,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Brands.AddAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Brand created successfully. BrandId={BrandId}",
            brand.BrandId);

        return new BrandResponseModel
        {
            BrandId = brand.BrandId,
            BrandName = brand.BrandName,
            BrandCode = brand.BrandCode,
            BrandImage = brand.BrandImage,
            IsActive = brand.IsActive
        };
    }

    public async Task<BrandResponseModel> UpdateAsync(
        int brandId,
        UpdateBrandRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Updating brand BrandId={BrandId}",
            brandId);

        var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(
            predicate: brand => brand.BrandId == brandId && brand.IsActive,
            selector: brand => brand,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (brand == null)
        {
            _logger.LogWarning(
                "Update brand failed. Brand not found. BrandId={BrandId}",
                brandId);

            throw new NotFoundException($"Brand with ID {brandId} not found.");
        }


        if (!string.IsNullOrWhiteSpace(request.BrandCode))
        {
            
            var normalizedBrandCode = request.BrandCode.Trim();
            var isBrandCodeExist = await _unitOfWork.Brands.AnyAsync(
                predicate: otherBrand =>
                    otherBrand.BrandCode == normalizedBrandCode &&
                    otherBrand.BrandId != brandId,
                cancellationToken: cancellationToken);

            if (isBrandCodeExist)
            {
                _logger.LogWarning(
                    "Update brand failed. Duplicate BrandCode={BrandCode}",
                    normalizedBrandCode);

                throw new ConflictException("Brand code already exists.");
            }

            brand.BrandCode = normalizedBrandCode;
        }

        if (!string.IsNullOrWhiteSpace(request.BrandName))
        {
            
            var normalizedBrandName = request.BrandName.Trim();
            var isBrandNameExist = await _unitOfWork.Brands.AnyAsync(
                predicate: otherBrand =>
                    otherBrand.BrandName == normalizedBrandName &&
                    otherBrand.BrandId != brandId,
                cancellationToken: cancellationToken);

            if (isBrandNameExist)
            {
                _logger.LogWarning(
                    "Update brand failed. Duplicate BrandName={BrandName}",
                    normalizedBrandName);

                throw new ConflictException("Brand name already exists.");
            }

            brand.BrandName = normalizedBrandName;
        }

        if (request.BrandImageFile is { Length: > 0 })
        {
            var imageUrl = await _cloudinaryService.UploadFileAsync(
                request.BrandImageFile,
                folder: "sport-wear-shop/brands",
                cancellationToken: cancellationToken);

            brand.BrandImage = imageUrl;
        }

        if (request.IsActive.HasValue)
        {
            brand.IsActive = request.IsActive.Value;
        }

        brand.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Brands.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Brand updated successfully. BrandId={BrandId}",
            brandId);

        return new BrandResponseModel
        {
            BrandId = brand.BrandId,
            BrandName = brand.BrandName,
            BrandCode = brand.BrandCode,
            BrandImage = brand.BrandImage,
            IsActive = brand.IsActive
        };

    }
    public async Task DeleteAsync(
        int brandId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Deleting brand. BrandId={BrandId}",
            brandId);

        var brand = await _unitOfWork.Brands.FirstOrDefaultAsync(
            predicate: brand => brand.BrandId == brandId,
            selector: brand => brand,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (brand == null)
        {
            _logger.LogWarning(
                "Delete brand failed. Brand not found. BrandId={BrandId}",
                brandId);

            throw new NotFoundException($"Brand with ID {brandId} not found.");
        }
        if (!brand.IsActive)
        {
            _logger.LogWarning(
                "Delete brand failed. Brand has been deactive. BrandId={BrandId}",
                brandId);
            throw new ConflictException("Cannot delete brand while being deactive.");
        }


        var hasProducts = await _unitOfWork.Products.AnyAsync(
            predicate: product => product.BrandId == brand.BrandId
                                  && product.Status != ProductStatus.Deleted,
            cancellationToken: cancellationToken);

        if (hasProducts)
        {
            _logger.LogWarning(
                "Delete brand failed. Brand has active products. BrandId={BrandId}",
                brandId);
            throw new ConflictException("Cannot delete brand with active products.");
        }

        brand.IsActive = false;
        brand.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Brands.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Brand deleted successfully (soft delete). BrandId={BrandId}",
            brandId);
    }

    
}