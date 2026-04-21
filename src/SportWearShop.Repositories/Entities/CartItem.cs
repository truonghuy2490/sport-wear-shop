using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class CartItem
{
    public long CartItemId { get; set; }

    public long CartId { get; set; }

    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
