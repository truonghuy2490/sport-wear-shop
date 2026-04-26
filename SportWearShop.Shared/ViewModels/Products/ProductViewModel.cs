/*using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.Products;

public class ProductViewModel
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Gender { get; set; }

    public string? BaseMaterial { get; set; }

    public string Status { get; set; } = string.Empty;

    public string BrandName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? ThumbnailUrl { get; set; }

    public double AverageRating { get; set; }

    public int TotalStock { get; set; }
}
*/

// SportWearShop.Shared/ViewModels/Products/ProductDetailViewModel.cs
namespace SportWearShop.Shared.ViewModels.Products
{
    public class ProductDetailViewModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Gender { get; set; }
        public string? BaseMaterial { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalStock { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; } = new();
        public List<ProductImageViewModel> Images { get; set; } = new();
        public List<ProductRatingViewModel> Ratings { get; set; } = new();
    }

    public class ProductVariantViewModel
    {
        public long ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string SizeCode { get; set; } = string.Empty;
        public string SizeLabel { get; set; } = string.Empty;
        public decimal ListPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class ProductImageViewModel
    {
        public long ProductImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class ProductRatingViewModel
    {
        public long ProductRatingId { get; set; }
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RatingValue { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    // Add missing properties to ProductViewModel
    public class ProductViewModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Gender { get; set; }
        public string? BaseMaterial { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public int BrandId { get; set; }  // Add this
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }  // Add this
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double AverageRating { get; set; }
        public int TotalStock { get; set; }
    }

    public class ProductQueryViewModel
    {
        public string? Keyword { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    
}