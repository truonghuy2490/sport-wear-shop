using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.BusinessLogics.Interfaces
{
    public interface IProductService
    {
        Task<PagingResponseModel<ProductResponseModel>> GetAllAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<ProductDetailResponseModel> GetDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default);
    }
}