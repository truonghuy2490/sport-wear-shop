using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.BusinessLogics.Interfaces
{
    public interface IProductService
    {
        Task<PagingResponseModel<ProductResponseModel>> GetAllAsync(
            ProductQueryRequestModel request,
            CancellationToken cancellationToken = default);

        Task<ProductDetailResponseModel> GetDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default);

        Task<ProductResponseModel> CreateAsync(
            CreateProductRequestModel request,
            CancellationToken cancellationToken = default);

        Task<ProductResponseModel> UpdateAsync(
            long productId,
            UpdateProductRequestModel request,
            CancellationToken cancellationToken = default);


        Task DeleteAsync(
            long productId,
            CancellationToken cancellationToken = default);

        Task<AdminProductDetailResponseModel> GetAdminDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default);     
    }


}