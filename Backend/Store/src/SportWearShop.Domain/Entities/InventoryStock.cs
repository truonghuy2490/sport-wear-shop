namespace SportWearShop.Domain.Entities;

public sealed class InventoryStock : IUpdatedAtUtc
{
    public long ProductVariantId { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
