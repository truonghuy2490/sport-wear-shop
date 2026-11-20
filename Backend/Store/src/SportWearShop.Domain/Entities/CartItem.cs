namespace SportWearShop.Domain.Entities;

public sealed class CartItem : IUpdatedAtUtc
{
    public long CartItemId { get; set; }
    public long CartId { get; set; }
    public long ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
