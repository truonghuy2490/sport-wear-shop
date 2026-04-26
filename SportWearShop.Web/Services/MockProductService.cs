// SportWearShop.Web/Services/MockProductService.cs
using SportWearShop.Shared.ViewModels.PageViewModel;
using SportWearShop.Shared.ViewModels.Products;
using SportWearShop.Web.Services.Interfaces;
using System.Collections.Generic;

namespace SportWearShop.Web.Services
{
    public class MockProductService : IProductMockService
    {
        private readonly List<ProductViewModel> _products;
        private readonly Dictionary<long, ProductDetailViewModel> _productDetails;

        public MockProductService()
        {
            _products = InitializeProducts();
            _productDetails = InitializeProductDetails();
        }

        public async Task<PagingViewModel<ProductViewModel>> GetPagedProductsAsync(
            ProductQueryViewModel query)
        {
            // Simulate async operation

            var queryable = _products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower();
                queryable = queryable.Where(p =>
                    p.ProductName.ToLower().Contains(keyword) ||
                    p.BrandName.ToLower().Contains(keyword));
            }

            if (query.BrandId.HasValue)
            {
                queryable = queryable.Where(p => p.BrandId == query.BrandId.Value);
            }

            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Gender))
            {
                queryable = queryable.Where(p => p.Gender == query.Gender);
            }

            if (query.MinPrice.HasValue)
            {
                queryable = queryable.Where(p => p.MinPrice >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p => p.MaxPrice <= query.MaxPrice.Value);
            }

            var totalCount = queryable.Count();
            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var items = queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagingViewModel<ProductViewModel>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<ProductDetailViewModel> GetProductByIdAsync(long id)
        {
            await Task.Delay(100);
            return _productDetails.TryGetValue(id, out var product)
                ? product
                : null;
        }

        public async Task<ProductDetailViewModel> GetProductBySlugAsync(string slug)
        {
            await Task.Delay(100);
            return _productDetails.Values.FirstOrDefault(p => p.Slug == slug);
        }

        public async Task<List<ProductViewModel>> GetProductsByBrandAsync(int brandId, int limit = 10)
        {
            await Task.Delay(200);
            return _products
                .Where(p => p.BrandId == brandId)
                .Take(limit)
                .ToList();
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(long productId, int limit = 5)
        {
            await Task.Delay(200);
            var currentProduct = _products.FirstOrDefault(p => p.ProductId == productId);

            if (currentProduct == null)
                return new List<ProductViewModel>();

            return _products
                .Where(p => p.ProductId != productId &&
                           (p.CategoryId == currentProduct.CategoryId ||
                            p.BrandId == currentProduct.BrandId))
                .Take(limit)
                .ToList();
        }

        #region Initialize Data

        private List<ProductViewModel> InitializeProducts()
        {
            return new List<ProductViewModel>
            {
                new() {
                    ProductId = 1,
                    ProductName = "Nike Air Zoom Pegasus 40",
                    ProductCode = "NIKE-AIR-ZOOM-PEGASUS-40",
                    Slug = "nike-air-zoom-pegasus-40",
                    Description = "Daily running shoes with responsive cushioning.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "Nike",
                    BrandId = 1,
                    CategoryName = "Running Shoes",
                    CategoryId = 1,
                    MinPrice = 2990000,
                    MaxPrice = 3200000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.5,
                    TotalStock = 50
                },
                new() {
                    ProductId = 2,
                    ProductName = "Adidas Ultraboost Light",
                    ProductCode = "ADIDAS-ULTRABOOST-LIGHT",
                    Slug = "adidas-ultraboost-light",
                    Description = "Premium running shoes with soft boost foam.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "Adidas",
                    BrandId = 2,
                    CategoryName = "Running Shoes",
                    CategoryId = 1,
                    MinPrice = 3290000,
                    MaxPrice = 3500000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.8,
                    TotalStock = 35
                },
                new() {
                    ProductId = 3,
                    ProductName = "PUMA Velocity Nitro",
                    ProductCode = "PUMA-VELOCITY-NITRO",
                    Slug = "puma-velocity-nitro",
                    Description = "Lightweight and stable shoes for gym sessions.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "PUMA",
                    BrandId = 3,
                    CategoryName = "Training Shoes",
                    CategoryId = 2,
                    MinPrice = 2590000,
                    MaxPrice = 2800000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.3,
                    TotalStock = 60
                },
                new() {
                    ProductId = 4,
                    ProductName = "Under Armour Curry Splash",
                    ProductCode = "UA-CURRY-SPLASH",
                    Slug = "under-armour-curry-splash",
                    Description = "Basketball shoes inspired by quick court movement.",
                    Gender = "MEN",
                    Status = "ACTIVE",
                    BrandName = "Under Armour",
                    BrandId = 4,
                    CategoryName = "Basketball Shoes",
                    CategoryId = 3,
                    MinPrice = 2890000,
                    MaxPrice = 3100000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.6,
                    TotalStock = 25
                },
                new() {
                    ProductId = 5,
                    ProductName = "New Balance 574 Classic",
                    ProductCode = "NB-574-CLASSIC",
                    Slug = "new-balance-574-classic",
                    Description = "Iconic lifestyle sneakers for daily wear.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "New Balance",
                    BrandId = 5,
                    CategoryName = "Sneakers",
                    CategoryId = 4,
                    MinPrice = 2450000,
                    MaxPrice = 2600000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.7,
                    TotalStock = 80
                },
                new() {
                    ProductId = 6,
                    ProductName = "ASICS Gel-Kayano 30",
                    ProductCode = "ASICS-GEL-KAYANO-30",
                    Slug = "asics-gel-kayano-30",
                    Description = "Supportive running shoes with gel cushioning.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "ASICS",
                    BrandId = 6,
                    CategoryName = "Running Shoes",
                    CategoryId = 1,
                    MinPrice = 3390000,
                    MaxPrice = 3600000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.9,
                    TotalStock = 30
                },
                new() {
                    ProductId = 7,
                    ProductName = "Reebok Nano X3",
                    ProductCode = "REEBOK-NANO-X3",
                    Slug = "reebok-nano-x3",
                    Description = "Versatile cross-training shoes for strength and cardio.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "Reebok",
                    BrandId = 7,
                    CategoryName = "Training Shoes",
                    CategoryId = 2,
                    MinPrice = 2750000,
                    MaxPrice = 2900000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.4,
                    TotalStock = 45
                },
                new() {
                    ProductId = 8,
                    ProductName = "FILA Disruptor II",
                    ProductCode = "FILA-DISRUPTOR-II",
                    Slug = "fila-disruptor-ii",
                    Description = "Chunky sneakers with bold streetwear style.",
                    Gender = "WOMEN",
                    Status = "ACTIVE",
                    BrandName = "FILA",
                    BrandId = 8,
                    CategoryName = "Sneakers",
                    CategoryId = 4,
                    MinPrice = 2150000,
                    MaxPrice = 2300000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.2,
                    TotalStock = 55
                },
                new() {
                    ProductId = 9,
                    ProductName = "Converse Chuck 70",
                    ProductCode = "CONVERSE-CHUCK-70",
                    Slug = "converse-chuck-70",
                    Description = "Classic canvas sneakers with timeless design.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "Converse",
                    BrandId = 9,
                    CategoryName = "Sneakers",
                    CategoryId = 4,
                    MinPrice = 1990000,
                    MaxPrice = 2100000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.5,
                    TotalStock = 100
                },
                new() {
                    ProductId = 10,
                    ProductName = "Vans Old Skool",
                    ProductCode = "VANS-OLD-SKOOL",
                    Slug = "vans-old-skool",
                    Description = "Skate-inspired sneakers for daily casual use.",
                    Gender = "UNISEX",
                    Status = "ACTIVE",
                    BrandName = "Vans",
                    BrandId = 10,
                    CategoryName = "Sneakers",
                    CategoryId = 4,
                    MinPrice = 1890000,
                    MaxPrice = 2000000,
                    ThumbnailUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3",
                    AverageRating = 4.6,
                    TotalStock = 120
                }
            };
        }

        private Dictionary<long, ProductDetailViewModel> InitializeProductDetails()
        {
            var details = new Dictionary<long, ProductDetailViewModel>();

            // Sample detail for product 1
            details[1] = new ProductDetailViewModel
            {
                ProductId = 1,
                ProductName = "Nike Air Zoom Pegasus 40",
                ProductCode = "NIKE-AIR-ZOOM-PEGASUS-40",
                Slug = "nike-air-zoom-pegasus-40",
                Description = "The Nike Air Zoom Pegasus 40 features responsive Zoom Air units and a lightweight mesh upper for breathable comfort. Ideal for daily running and training.",
                Gender = "UNISEX",
                BaseMaterial = "Mesh, Synthetic",
                Status = "ACTIVE",
                BrandName = "Nike",
                CategoryName = "Running Shoes",
                AverageRating = 4.5,
                TotalStock = 50,
                Variants = new List<ProductVariantViewModel>
                {
                    new() { SizeLabel = "US 7", ColorName = "Black/White", SalePrice = 2990000, ListPrice = 3200000, StockQuantity = 10 },
                    new() { SizeLabel = "US 8", ColorName = "Black/White", SalePrice = 2990000, ListPrice = 3200000, StockQuantity = 15 },
                    new() { SizeLabel = "US 9", ColorName = "Black/White", SalePrice = 2990000, ListPrice = 3200000, StockQuantity = 20 },
                    new() { SizeLabel = "US 10", ColorName = "Black/White", SalePrice = 2990000, ListPrice = 3200000, StockQuantity = 5 }
                },
                Images = new List<ProductImageViewModel>
                {
                    new() { ImageUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3", IsPrimary = true, SortOrder = 1 },
                    new() { ImageUrl = "https://ts4.tc.mm.bing.net/th/id/OIP-C.1E6oiqqmIc4koOeJ7hX8PAHaHa?rs=1&pid=ImgDetMain&o=7&rm=3", IsPrimary = false, SortOrder = 2 }
                },
                Ratings = new List<ProductRatingViewModel>
                {
                    new() { UserName = "John D.", RatingValue = 5, ReviewText = "Excellent running shoes!", CreatedAtUtc = DateTime.UtcNow.AddDays(-5) },
                    new() { UserName = "Sarah M.", RatingValue = 4, ReviewText = "Very comfortable but a bit pricey", CreatedAtUtc = DateTime.UtcNow.AddDays(-10) }
                }
            };

            // Add similar details for other products...
            for (int i = 2; i <= 10; i++)
            {
                var baseProduct = _products.First(p => p.ProductId == i);
                details[i] = new ProductDetailViewModel
                {
                    ProductId = baseProduct.ProductId,
                    ProductName = baseProduct.ProductName,
                    ProductCode = baseProduct.ProductCode,
                    Slug = baseProduct.Slug,
                    Description = baseProduct.Description + " Experience ultimate comfort and style with these premium sportswear shoes.",
                    Gender = baseProduct.Gender,
                    Status = baseProduct.Status,
                    BrandName = baseProduct.BrandName,
                    CategoryName = baseProduct.CategoryName,
                    AverageRating = baseProduct.AverageRating,
                    TotalStock = baseProduct.TotalStock,
                    Variants = new List<ProductVariantViewModel>
                    {
                        new() { SizeLabel = "US 7", ColorName = "Default", SalePrice = baseProduct.MinPrice, ListPrice = baseProduct.MaxPrice.Value, StockQuantity = 10 },
                        new() { SizeLabel = "US 8", ColorName = "Default", SalePrice = baseProduct.MinPrice, ListPrice = baseProduct.MaxPrice.Value, StockQuantity = 15 },
                        new() { SizeLabel = "US 9", ColorName = "Default", SalePrice = baseProduct.MinPrice, ListPrice = baseProduct.MaxPrice.Value, StockQuantity = 20 }
                    },
                    Images = new List<ProductImageViewModel>
                    {
                        new() { ImageUrl = "/images/product-placeholder.jpg", IsPrimary = true, SortOrder = 1 }
                    },
                    Ratings = new List<ProductRatingViewModel>()
                };
            }

            return details;
        }

        #endregion
    }
}