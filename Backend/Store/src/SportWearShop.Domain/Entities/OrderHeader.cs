namespace SportWearShop.Domain.Entities;

public sealed class OrderHeader : IUpdatedAtUtc
{
    public long OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public long CustomerId { get; set; }
    public string ShippingAddressSnapshot { get; set; } = null!;
    public string? BillingAddressSnapshot { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public DateTime OrderedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
