using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class InventoryMovement
{
    public long InventoryMovementId { get; set; }

    public long ProductVariantId { get; set; }

    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ReferenceType { get; set; }

    public long? ReferenceId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
