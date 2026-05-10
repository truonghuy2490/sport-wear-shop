using SportWearShop.Shared.ViewModels.ProductImages;
using SportWearShop.Shared.ViewModels.ProductRatings;
using SportWearShop.Shared.ViewModels.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.Products;

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