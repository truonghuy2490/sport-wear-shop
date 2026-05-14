using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.Services.Implementations;

public class ProductApiService : IProductApiService
{
    private readonly ApiClient _apiClient;

    public ProductApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PagingResponseModel<ProductResponseModel>?> GetAllAsync(
        ProductQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<KeyValuePair<string, string?>>
        {
            new("PageNumber", request.PageNumber.ToString()),
            new("PageSize", request.PageSize.ToString()),
            new("SortBy", ((int)request.SortBy).ToString()),
            new("IsAscending", request.IsAscending.ToString().ToLower())
        };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            queryParams.Add(new("SearchTerm", request.SearchTerm));

        if (request.Gender.HasValue)
            queryParams.Add(new("Gender", ((int)request.Gender.Value).ToString()));

        if (request.Status.HasValue)
            queryParams.Add(new("Status", ((int)request.Status.Value).ToString()));

        if (request.MinPrice.HasValue)
            queryParams.Add(new("MinPrice", request.MinPrice.Value.ToString(CultureInfo.InvariantCulture)));

        if (request.MaxPrice.HasValue)
            queryParams.Add(new("MaxPrice", request.MaxPrice.Value.ToString(CultureInfo.InvariantCulture)));

        if (request.IsNewRelease.HasValue)
            queryParams.Add(new("IsNewRelease", request.IsNewRelease.Value.ToString().ToLower()));

        if (request.IsOnSale.HasValue)
            queryParams.Add(new("IsOnSale", request.IsOnSale.Value.ToString().ToLower()));

        if (request.CreatedFromUtc.HasValue)
            queryParams.Add(new("CreatedFromUtc", request.CreatedFromUtc.Value.ToString("O")));

        if (request.CreatedToUtc.HasValue)
            queryParams.Add(new("CreatedToUtc", request.CreatedToUtc.Value.ToString("O")));

        if (request.BrandIds?.Any() == true)
        {
            foreach (var brandId in request.BrandIds)
                queryParams.Add(new("BrandIds", brandId.ToString()));
        }

        if (request.CategoryIds?.Any() == true)
        {
            foreach (var categoryId in request.CategoryIds)
                queryParams.Add(new("CategoryIds", categoryId.ToString()));
        }

        var endpoint = QueryHelpers.AddQueryString("api/products", queryParams);

        return await _apiClient.GetAsync<PagingResponseModel<ProductResponseModel>>(
            endpoint,
            cancellationToken);
    }

    public async Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<ProductDetailResponseModel>(
            $"api/products/{productId}",
            cancellationToken);
    }
}