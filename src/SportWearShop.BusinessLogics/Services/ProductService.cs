using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;

public class ProductService: IProductService
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

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            string keyword = query.Keyword.Trim();

            productQuery = productQuery.Where(p =>
                p.ProductName.Contains(keyword) ||
                (p.Description != null && p.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            string gender = query.Gender.Trim();
            productQuery = productQuery.Where(p => p.Gender == gender);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            string status = query.Status.Trim();
            productQuery = productQuery.Where(p => p.Status == status);
        }

        if (query.MinPrice.HasValue)
        {
            productQuery = productQuery.Where(p =>
                p.ProductVariants.Any(v => v.SalePrice >= query.MinPrice.Value));
        }

        if (query.MaxPrice.HasValue)
        {
            productQuery = productQuery.Where(p =>
                p.ProductVariants.Any(v => v.SalePrice <= query.MaxPrice.Value));
        }

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

                // -- 
                Status = p.Status,
                BrandName = p.Brand.BrandName,
                CategoryName = p.Category.CategoryName,

                // ---
                MinPrice = p.ProductVariants
                    .Select(v => (decimal?)v.SalePrice)
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
                    .Select(v => v.InventoryStock)
                    .Where(s => s != null)
                    .Sum(s => s.QuantityOnHand - s.QuantityReserved)
            })
            .ToListAsync(cancellationToken);

        return new PagingResponseModel<ProductResponseModel>(
             items,
             totalCount,
             pageNumber,
             pageSize
        );
    }

    public async Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var brandTask = _unitOfWork.Brands.GetByIdAsync(product.BrandId, cancellationToken);
        var categoryTask = _unitOfWork.Categories.GetByIdAsync(product.CategoryId, cancellationToken);
        var variantsTask = _unitOfWork.ProductVariants.FindAsync(x => x.ProductId == productId, cancellationToken: cancellationToken);
        var imagesTask = _unitOfWork.ProductImages.FindAsync(x => x.ProductId == productId, cancellationToken: cancellationToken);
        var ratingsTask = _unitOfWork.ProductRatings.FindAsync(x => x.ProductId == productId, cancellationToken: cancellationToken);

        await Task.WhenAll(brandTask!, categoryTask!, variantsTask, imagesTask, ratingsTask);

        var brand = await brandTask;
        var category = await categoryTask;
        var variants = (await variantsTask).ToList();
        var images = (await imagesTask).OrderByDescending(x => x.IsPrimary).ThenBy(x => x.SortOrder).ToList();
        var ratings = (await ratingsTask).OrderByDescending(x => x.CreatedAtUtc).ToList();

        var variantIds = variants.Select(x => x.ProductVariantId).ToList();

        var stocks = await _unitOfWork.InventoryStocks.FindAsync(
            x => variantIds.Contains(x.ProductVariantId),
            cancellationToken: cancellationToken);

        var stockLookup = stocks.ToDictionary(x => x.ProductVariantId, x => x);

        var variantModels = variants.Select(v => new ProductVariantResponseModel
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
            StockQuantity = stockLookup.TryGetValue(v.ProductVariantId, out var stock)
                ? stock.QuantityOnHand - stock.QuantityReserved
                : 0
        }).ToList();

        var imageModels = images.Select(i => new ProductImageResponseModel
        {
            ProductImageId = i.ProductImageId,
            ImageUrl = i.ImageUrl,
            AltText = i.AltText,
            SortOrder = i.SortOrder,
            IsPrimary = i.IsPrimary
        }).ToList();

        var ratingModels = ratings.Select(r => new ProductRatingResponseModel
        {
            ProductRatingId = r.ProductRatingId,
            UserId = r.UserId,
            RatingValue = r.RatingValue,
            ReviewText = r.ReviewText,
            CreatedAtUtc = r.CreatedAtUtc
        }).ToList();

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
            BrandName = brand?.BrandName ?? string.Empty,
            CategoryName = category?.CategoryName ?? string.Empty,
            AverageRating = ratingModels.Any() ? ratingModels.Average(x => x.RatingValue) : 0,
            TotalStock = variantModels.Sum(x => x.StockQuantity),
            Variants = variantModels,
            Images = imageModels,
            Ratings = ratingModels
        };
    }

    public async Task<long> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var brandExists = await _unitOfWork.Brands.AnyAsync(
            x => x.BrandId == request.BrandId,
            cancellationToken);

        if (!brandExists)
        {
            throw new InvalidOperationException("Brand does not exist.");
        }

        var categoryExists = await _unitOfWork.Categories.AnyAsync(
            x => x.CategoryId == request.CategoryId,
            cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Category does not exist.");
        }

        var productCodeExists = await _unitOfWork.Products.AnyAsync(
            x => x.ProductCode == request.ProductCode,
            cancellationToken);

        if (productCodeExists)
        {
            throw new InvalidOperationException("Product code already exists.");
        }

        var slugExists = await _unitOfWork.Products.AnyAsync(
            x => x.Slug == request.Slug,
            cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException("Slug already exists.");
        }

        var entity = new Product
        {
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            ProductName = request.ProductName,
            ProductCode = request.ProductCode,
            Slug = request.Slug,
            Description = request.Description,
            Gender = request.Gender,
            BaseMaterial = request.BaseMaterial,
            Status = request.Status
        };

        await _unitOfWork.Products.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created product {ProductId}", entity.ProductId);

        return entity.ProductId;
    }

    public async Task<bool> UpdateAsync(
        long productId,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        var brandExists = await _unitOfWork.Brands.AnyAsync(
            x => x.BrandId == request.BrandId,
            cancellationToken);

        if (!brandExists)
        {
            throw new InvalidOperationException("Brand does not exist.");
        }

        var categoryExists = await _unitOfWork.Categories.AnyAsync(
            x => x.CategoryId == request.CategoryId,
            cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Category does not exist.");
        }

        var duplicateProductCode = await _unitOfWork.Products.AnyAsync(
            x => x.ProductId != productId && x.ProductCode == request.ProductCode,
            cancellationToken);

        if (duplicateProductCode)
        {
            throw new InvalidOperationException("Product code already exists.");
        }

        var duplicateSlug = await _unitOfWork.Products.AnyAsync(
            x => x.ProductId != productId && x.Slug == request.Slug,
            cancellationToken);

        if (duplicateSlug)
        {
            throw new InvalidOperationException("Slug already exists.");
        }

        entity.BrandId = request.BrandId;
        entity.CategoryId = request.CategoryId;
        entity.ProductName = request.ProductName;
        entity.ProductCode = request.ProductCode;
        entity.Slug = request.Slug;
        entity.Description = request.Description;
        entity.Gender = request.Gender;
        entity.BaseMaterial = request.BaseMaterial;
        entity.Status = request.Status;

        _unitOfWork.Products.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated product {ProductId}", productId);

        return true;
    }

    public async Task<bool> DeleteAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Status = "INACTIVE";
        _unitOfWork.Products.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted product {ProductId}", productId);

        return true;
    }

    

}