using SportWearShop.BusinessLogics.ResponseModels.CategoryModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseModel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryDetailResponseModel?> GetByIdAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponseModel>> GetRootCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponseModel>> GetChildrenAsync(
        int parentCategoryId,
        CancellationToken cancellationToken = default);

    Task<CategoryResponseModel> CreateAsync(
        CategoryCreateRequestModel request,
        CancellationToken cancellationToken = default);

    Task<CategoryResponseModel> UpdateAsync(
        int categoryId,
        CategoryUpdateRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int categoryId,
        CancellationToken cancellationToken = default);
}