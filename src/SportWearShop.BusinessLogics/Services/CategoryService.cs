using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using SportWearShop.BusinessLogics.ResponseModels.CategoryModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace SportWearShop.BusinessLogics.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(
        ILogger<CategoryService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagingResponseModel<CategoryResponseModel>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving active categories. PageNumber={PageNumber}, PageSize={PageSize}",
            pageNumber,
            pageSize);

        var options = new QueryOptions<Category>
        {
            Filter = category => category.IsActive,
            SortBy = category => category.SortOrder,
            Ascending = true,
            PageNumber = pageNumber,
            PageSize = pageSize,
            AsNoTracking = true
        };

        var result = await _unitOfWork.Categories.FindWithPagingAsync(
            options,
            selector: category => new CategoryResponseModel
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                Description = category.Description,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            },
            cancellationToken);

        _logger.LogInformation(
            "Retrieved active categories successfully. ReturnedItems={ReturnedItems}, TotalCount={TotalCount}",
            result.Items.Count,
            result.TotalCount);

        return new PagingResponseModel<CategoryResponseModel>(
            result.Items,
            result.TotalCount,
            pageNumber,
            pageSize);
    }

    public async Task<CategoryDetailResponseModel?> GetByIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching category detail for CategoryId={CategoryId}", categoryId);
        var category = await _unitOfWork.Categories.FirstOrDefaultAsync(
            predicate: category => category.CategoryId == categoryId && category.IsActive,
            selector: category => new CategoryDetailResponseModel
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                Description = category.Description,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive,

                Children = category.InverseParentCategory
                    .Where(child => child.IsActive)
                    .OrderBy(child => child.SortOrder)
                    .Select(child => new CategoryResponseModel
                    {
                        CategoryId = child.CategoryId,
                        ParentCategoryId = child.ParentCategoryId,
                        CategoryCode = child.CategoryCode,
                        CategoryName = child.CategoryName,
                        Description = child.Description,
                        SortOrder = child.SortOrder,
                        IsActive = child.IsActive
                    })
                    .ToList()
            },
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Category, object>>[]
            {
                category => category.InverseParentCategory
            }
         );
        if (category == null)
        {
            _logger.LogWarning("Category not found. CategoryId={CategoryId}", categoryId);
            throw new NotFoundException($"Category with ID {categoryId} not found.");
        }
        _logger.LogInformation("Successfully retrieved category detail for CategoryId={CategoryId}", categoryId);
        return category;
    }
    
    public async Task<PagingResponseModel<CategoryResponseModel>> GetRootCategoriesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var options = new QueryOptions<Category>
        {
            Filter = category => category.ParentCategoryId == null
                                && category.IsActive,
            SortBy = category => category.SortOrder,
            Ascending = true,
            PageNumber = pageNumber,
            PageSize = pageSize,
            AsNoTracking = true
        };

        var result = await _unitOfWork.Categories.FindWithPagingAsync(
            options,
            selector: category => new CategoryResponseModel
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryName = category.CategoryName,
                CategoryCode = category.CategoryCode,
                Description = category.Description,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            },
            cancellationToken);

        return new PagingResponseModel<CategoryResponseModel>(
            result.Items,
            result.TotalCount,
            pageNumber,
            pageSize);
    }

    public async Task<List<CategoryResponseModel>> GetChildrenAsync(
        int parentCategoryId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Categories.FindAsync(
            filter: category => category.ParentCategoryId == parentCategoryId
                                && category.IsActive,
            selector: category => new CategoryResponseModel
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryName = category.CategoryName,
                CategoryCode = category.CategoryCode,
                Description = category.Description,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            },
            sortBy: category => category.SortOrder,
            ascending: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);
    }

    public async Task<CategoryResponseModel> CreateAsync(
        CreateCategoryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation(
            "Creating new category with Code={CategoryCode}, Name={CategoryName}",
            request.CategoryCode,
            request.CategoryName);

        var isCategoryExist = await _unitOfWork.Categories.AnyAsync(
            predicate: category => category.CategoryCode == request.CategoryCode && category.IsActive,
            cancellationToken: cancellationToken
        );

        if (isCategoryExist) {
            _logger.LogWarning(
                "Create category failed. Duplicate CategoryCode={CategoryCode}",
                request.CategoryCode);

            throw new ConflictException("Category code already exists."); 
        }

        if (request.ParentCategoryId.HasValue)
        {

            var isParentCategoryExist = await _unitOfWork.Categories.AnyAsync(
                predicate: category => category.CategoryId == request.ParentCategoryId && category.IsActive,
                cancellationToken: cancellationToken
            );

            if (!isParentCategoryExist) {
                _logger.LogWarning(
                    "Create category failed. Parent category with ID {ParentCategoryId} not found.",
                    request.ParentCategoryId);
                throw new NotFoundException($"Parent category with ID {request.ParentCategoryId} not found."); 
            }
        }

        var category = new Category
        {
            ParentCategoryId = request.ParentCategoryId,
            CategoryName = request.CategoryName,
            CategoryCode = request.CategoryCode,
            Description = request.Description,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category created successfully. CategoryId={CategoryId}",
            category.CategoryId);

        return new CategoryResponseModel
        {
            CategoryId = category.CategoryId,
            ParentCategoryId = category.ParentCategoryId,
            CategoryName = category.CategoryName,
            CategoryCode = category.CategoryCode,
            Description = category.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        };
    }

    public async Task<CategoryResponseModel> UpdateAsync(
        int categoryId,
        UpdateCategoryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Updating category CategoryId={CategoryId}",
            categoryId);

        var category = await _unitOfWork.Categories.FirstOrDefaultAsync(
            predicate: category => category.CategoryId == categoryId,
            selector: category => category,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (category == null)
        {
            _logger.LogWarning(
                "Update category failed. Category not found. CategoryId={CategoryId}",
                categoryId);

            throw new NotFoundException($"Category with ID {categoryId} not found.");
        }

        request.CategoryName = request.CategoryName.Trim();
        request.CategoryCode = request.CategoryCode.Trim();

        var isCategoryCodeExist = await _unitOfWork.Categories.AnyAsync(
            predicate: category =>
                category.CategoryId != categoryId &&
                category.CategoryCode == request.CategoryCode &&
                category.IsActive,
            cancellationToken: cancellationToken);

        if (isCategoryCodeExist)
        {
            _logger.LogWarning(
                "Update category failed. Duplicate CategoryCode={CategoryCode}",
                request.CategoryCode);

            throw new ConflictException("Category code already exists.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == category.CategoryId)
            {
                _logger.LogWarning(
                    "Update category failed. Category cannot be parent of itself. CategoryId={CategoryId}",
                    categoryId);

                throw new ConflictException("Category cannot be parent of itself.");
            }

            var parentExists = await _unitOfWork.Categories.AnyAsync(
                predicate: category =>
                    category.CategoryId == request.ParentCategoryId.Value &&
                    category.IsActive,
                cancellationToken: cancellationToken);

            if (!parentExists)
            {
                _logger.LogWarning(
                    "Update category failed. Parent category not found. ParentCategoryId={ParentCategoryId}",
                    request.ParentCategoryId);

                throw new NotFoundException("Parent category does not exist.");
            }
        }

        category.ParentCategoryId = request.ParentCategoryId;
        category.CategoryName = request.CategoryName;
        category.CategoryCode = request.CategoryCode;
        category.Description = request.Description;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category updated successfully. CategoryId={CategoryId}",
            categoryId);

        return new CategoryResponseModel
        {
            CategoryId = category.CategoryId,
            ParentCategoryId = category.ParentCategoryId,
            CategoryName = category.CategoryName,
            CategoryCode = category.CategoryCode,
            Description = category.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        };
    }
    public async Task DeleteAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
           "Deleting category. CategoryId={CategoryId}",
           categoryId);

        var category = await _unitOfWork.Categories.FirstOrDefaultAsync(
            predicate: category => category.CategoryId == categoryId && category.IsActive,
            selector: category => category,
            cancellationToken: cancellationToken);

        if (category == null)
        {
            _logger.LogWarning(
                "Delete category failed. Category not found. CategoryId={CategoryId}",
                categoryId);

            throw new NotFoundException($"Category with ID {categoryId} not found.");
        }

        var hasChildren = await _unitOfWork.Categories.AnyAsync(
            predicate: child => child.ParentCategoryId == category.CategoryId && child.IsActive,
            cancellationToken: cancellationToken);

        if (hasChildren)
        {
            _logger.LogWarning(
                "Delete category failed. Category has active child categories. CategoryId={CategoryId}",
                categoryId);

            throw new ConflictException("Cannot delete category with active child categories.");
        }

        var hasProducts = await _unitOfWork.Products.AnyAsync(
            predicate: product => product.CategoryId == category.CategoryId && product.Status == ProductStatus.Active,
            cancellationToken: cancellationToken);

        if (hasProducts)
        {
            _logger.LogWarning(
                "Delete category failed. Category has active products. CategoryId={CategoryId}",
                categoryId);

            throw new ConflictException("Cannot delete category with active products.");
        }

        category.IsActive = false;  
        category.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);    
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category deleted successfully (soft delete). CategoryId={CategoryId}",
            categoryId);
    }

    public async Task<List<CategoryTreeResponseModel>> GetTreeAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.FindAsync(
            filter: category => category.IsActive,
            selector: category => new CategoryTreeResponseModel
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryName = category.CategoryName,
                CategoryCode = category.CategoryCode,
                SortOrder = category.SortOrder
            },
            sortBy: category => category.SortOrder,
            ascending: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        var lookup = categories.ToLookup(category => category.ParentCategoryId);

        List<CategoryTreeResponseModel> BuildTree(int? parentId)
        {
            return lookup[parentId]
                .OrderBy(category => category.SortOrder)
                .Select(category =>
                {
                    category.Children = BuildTree(category.CategoryId);
                    return category;
                })
                .ToList();
        }

        return BuildTree(null);
    }
}