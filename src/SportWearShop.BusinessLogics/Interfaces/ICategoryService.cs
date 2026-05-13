using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using SportWearShop.BusinessLogics.ResponseModels.CategoryModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface ICategoryService
{
    Task<PagingResponseModel<CategoryResponseModel>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<CategoryDetailResponseModel?> GetByIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default);
    
    Task<List<CategoryTreeResponseModel>> GetTreeAsync(
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponseModel>> GetRootCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponseModel>> GetChildrenAsync(
        int parentCategoryId,
        CancellationToken cancellationToken = default);

    Task<CategoryResponseModel> CreateAsync(
        CreateCategoryRequestModel request,
        CancellationToken cancellationToken = default);

    Task<CategoryResponseModel> UpdateAsync(
        int categoryId,
        UpdateCategoryRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int categoryId,
        CancellationToken cancellationToken = default);
}