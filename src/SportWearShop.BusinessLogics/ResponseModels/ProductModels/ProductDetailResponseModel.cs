using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels
{
    public class ProductDetailResponseModel
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

        public List<ProductVariantResponseModel> Variants { get; set; } = new();

        public List<ProductImageResponseModel> Images { get; set; } = new();

        public List<ProductRatingResponseModel> Ratings { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
