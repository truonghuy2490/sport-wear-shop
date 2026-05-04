using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace SportWearShop.BusinessLogics.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagingResponseModel<ProductResponseModel>> GetAllAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            var totalCount = await _unitOfWork.Products.CountAsync(
                product => product.Status == "ACTIVE",
                cancellationToken);

            var products = await _unitOfWork.Products.FindAsync(
                filter: product => product.Status == "ACTIVE",
                selector: product => new ProductResponseModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Slug = product.Slug,
                    BrandName = product.Brand.BrandName,
                    CategoryName = product.Category.CategoryName,
                    Status = product.Status,
                    CreatedAtUtc = product.CreatedAtUtc
                },
                sortBy: product => product.CreatedAtUtc,
                ascending: false,
                asNoTracking: true,
                cancellationToken: cancellationToken,
                includes: new Expression<Func<Product, object>>[]
                {
                    product => product.Brand,
                    product => product.Category
                }
            );

            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagingResponseModel<ProductResponseModel>(
                pagedProducts,
                totalCount,
                pageNumber,
                pageSize);
        }

        public async Task<ProductDetailResponseModel> GetDetailsAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(
                predicate: product => product.ProductId == productId
                                      && product.Status == "ACTIVE",
                selector: product => new ProductDetailResponseModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Slug = product.Slug,
                    Description = product.Description,
                    Gender = product.Gender,
                    BaseMaterial = product.BaseMaterial,
                    BrandName = product.Brand.BrandName,
                    CategoryName = product.Category.CategoryName,
                    Status = product.Status,
                    CreatedAtUtc = product.CreatedAtUtc,
                    UpdatedAtUtc = product.UpdatedAtUtc
                },
                asNoTracking: true,
                cancellationToken: cancellationToken,
                includes: new Expression<Func<Product, object>>[]
                {
                    product => product.Brand,
                    product => product.Category
                }
            );

            if (product == null)
            {
                throw new NotFoundException($"Product with id {productId} was not found.");
            }

            return product;
        }
    }
}