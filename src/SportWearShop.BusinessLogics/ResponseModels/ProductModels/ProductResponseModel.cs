using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels
{
    public class ProductResponseModel
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

        public DateTime CreatedAtUtc { get; set; }
    }
}
