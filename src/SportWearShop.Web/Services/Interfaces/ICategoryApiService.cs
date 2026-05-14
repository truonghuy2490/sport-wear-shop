using SportWearShop.Shared.ViewModels.CategoryModels;

namespace SportWearShop.Web.Services.Interfaces;

public interface ICategoryApiService
{
    Task<List<CategoryTreeResponseModel>> GetTreeAsync(
        CancellationToken cancellationToken = default);
}