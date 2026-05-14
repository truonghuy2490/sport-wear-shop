using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.InventoryModels;

public class InventoryStockResponseModel
{
    public long ProductVariantId { get; set; }

    public string Sku { get; set; } = null!;

    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    public int AvailableStock => QuantityOnHand - QuantityReserved;

    public DateTime UpdatedAtUtc { get; set; }
}