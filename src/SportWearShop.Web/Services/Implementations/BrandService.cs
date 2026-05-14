using Microsoft.AspNetCore.WebUtilities;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.BrandModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class BrandApiService : IBrandApiService
{
    private readonly ApiClient _apiClient;

    public BrandApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PagingResponseModel<BrandResponseModel>?> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        var endpoint = QueryHelpers.AddQueryString(
            "api/brands",
            queryParams);

        return await _apiClient.GetAsync<PagingResponseModel<BrandResponseModel>>(
            endpoint,
            cancellationToken);
    }
}