<<<<<<< HEAD
using SportWearShop.Shared.ViewModels;
using SportWearShop.Shared.ViewModels.ProductModels;
=======
using SportWearShop.Shared.ViewModels.Products;
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653

namespace SportWearShop.Web.Services.Interfaces;

/// <summary>
/// product api service
/// </summary>
/// 
public interface IProductApiService
<<<<<<< HEAD
{
    Task<PagingResponseModel<ProductResponseModel>?> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponseModel?> GetByIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<ProductResponseModel?> CreateAsync(
        CreateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task<ProductResponseModel?> UpdateAsync(
        long productId,
        UpdateProductRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long productId,
        CancellationToken cancellationToken = default);
}
=======
{/*
    Task<List<ProductViewModel>> GetProductsAsync();
    Task<ProductViewModel> GetProductByIdAsync(int id);*/
}
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
