using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using System;
using System.Collections.Generic;
using System.Text;
namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels;

public class ProductDetailResponseModel
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string Gender { get; set; } = null!;

    public string? BaseMaterial { get; set; }

    public string Status { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public List<ProductImageResponseModel> Images { get; set; } = new();

    public List<ProductVariantResponseModel> Variants { get; set; } = new();
}