using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;

public class ProductVariantResponseModel
{
    public long ProductVariantId { get; set; }

    public string Sku { get; set; } = null!;

    public string ColorCode { get; set; } = null!;

    public string ColorName { get; set; } = null!;

    public string SizeCode { get; set; } = null!;

    public string SizeLabel { get; set; } = null!;

    public decimal ListPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public int? WeightGrams { get; set; }

    public string Status { get; set; } = null!;

    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    public int AvailableStock => QuantityOnHand - QuantityReserved;

    public List<ProductImageResponseModel> Images { get; set; } = new();
}