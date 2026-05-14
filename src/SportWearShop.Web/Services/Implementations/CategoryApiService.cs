using SportWearShop.Shared.ViewModels.CategoryModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class CategoryApiService : ICategoryApiService
{
    private readonly ApiClient _apiClient;

    public CategoryApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CategoryTreeResponseModel>> GetTreeAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _apiClient.GetAsync<List<CategoryTreeResponseModel>>(
            "api/categories/tree",
            cancellationToken);

        return result ?? [];
    }
}