using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.ProductImages;

public class ProductImageViewModel
{
    public long ProductImageId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}