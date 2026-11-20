namespace SportWearShop.Domain.Entities;

public sealed class OrderItem
{
    public long OrderItemId { get; set; }
    public long OrderId { get; set; }
    public long? ProductVariantId { get; set; }
    public string SkuSnapshot { get; set; } = null!;
    public string ProductNameSnapshot { get; set; } = null!;
    public string ColorSnapshot { get; set; } = null!;
    public string SizeSnapshot { get; set; } = null!;
    public string? ImageUrlSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineDiscountAmount { get; set; }
    public decimal LineTotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
