namespace SportWearShop.BusinessLogics.ResponseModels.InventoryModels;

public class InventoryMovementResponseModel
{
    public long InventoryMovementId { get; set; }

    public long ProductVariantId { get; set; }
    public string Sku {get;set;} = null!;

    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ReferenceType { get; set; }

    public long? ReferenceId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}