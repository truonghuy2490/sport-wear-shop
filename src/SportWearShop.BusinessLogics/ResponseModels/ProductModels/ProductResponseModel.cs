using System;
using System.Collections.Generic;
using System.Text;
namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels;

public class ProductResponseModel
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public decimal MinPrice { get; set; }

    public decimal? MinSalePrice { get; set; }

    public int TotalVariants { get; set; }
}
