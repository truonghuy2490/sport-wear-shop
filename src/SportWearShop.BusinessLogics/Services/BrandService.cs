using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
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

    public BrandService(
        IUnitOfWork unitOfWork,
        ILogger<BrandService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<BrandResponseModel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var brands = await _unitOfWork.Brands.FindAsync(
            filter: brand => brand.IsActive,
            selector: brand => new BrandResponseModel
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                BrandCode = brand.BrandCode,
                BrandImage = brand.BrandImage,
                IsActive = brand.IsActive
            },
            sortBy: brands => brands.BrandName,
            ascending: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (!brands.Any())
        {
            _logger.LogWarning("No active brands found.");
            throw new NotFoundException("No active brands found.");
        }

        _logger.LogInformation("Retrieved {Count} active brands.", brands.Count);
        return brands;
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
            // have some problem with adidas original and adidas performance 
            // maybe ADIDAS_ORIGINAL and ADIDAS_PERFORMANCE will be better for brand code
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

        var brand = new Brand
        {
            BrandName = normalizedBrandName,
            BrandCode = normalizedBrandCode,
            BrandImage = request.BrandImage,
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

        var normalizedBrandCode = request.BrandCode.Trim().ToUpper();
        var normalizedBrandName = request.BrandName.Trim();

        var isBrandCodeExist = await _unitOfWork.Brands.AnyAsync(
            predicate: brand => brand.BrandCode == normalizedBrandCode
                                && brand.BrandId != brandId
                                && brand.IsActive,
            cancellationToken: cancellationToken);

        if (isBrandCodeExist)
        {
            _logger.LogWarning(
                "Update brand failed. Duplicate BrandCode={BrandCode}",
                normalizedBrandCode);

            throw new ConflictException("Brand code already exists.");
        }

        var isBrandNameExist = await _unitOfWork.Brands.AnyAsync(
            predicate: brand => brand.BrandName == normalizedBrandName
                                && brand.BrandId != brandId
                                && brand.IsActive,
            cancellationToken: cancellationToken);

        if (isBrandNameExist)
        {
            _logger.LogWarning(
                "Update brand failed. Duplicate BrandName={BrandName}",
                normalizedBrandName);

            throw new ConflictException("Brand name already exists.");
        }

        brand.BrandName = normalizedBrandName;
        brand.BrandCode = normalizedBrandCode;
        brand.BrandImage = request.BrandImage;
        brand.IsActive = request.IsActive;
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
            predicate: brand => brand.BrandId == brandId && brand.IsActive,
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

        var hasProducts = await _unitOfWork.Products.AnyAsync(
            predicate: product => product.BrandId == brand.BrandId
                                  && product.Status == ProductStatus.Active,
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