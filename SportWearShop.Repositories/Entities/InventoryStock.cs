using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class InventoryStock
{
    public long ProductVariantId { get; set; }

    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
