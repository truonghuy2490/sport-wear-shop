using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.BrandModels;

namespace SportWearShop.Web.Services.Interfaces;

public interface IBrandApiService
{
    Task<PagingResponseModel<BrandResponseModel>?> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default);
}