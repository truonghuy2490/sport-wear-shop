using System;
using System.Collections.Generic;
using SportWearShop.Repositories.Enums;

namespace SportWearShop.Repositories.Entities;

public partial class InventoryMovement
{
    public long InventoryMovementId { get; set; }

    public long ProductVariantId { get; set; }

    public InventoryMovementType MovementType { get; set; } 

    public int Quantity { get; set; }

    public InventoryReferenceType ReferenceType { get; set; }

    public long? ReferenceId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
