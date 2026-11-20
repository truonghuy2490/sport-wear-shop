namespace SportWearShop.Domain.Entities;

public sealed class PaymentTransaction : IUpdatedAtUtc
{
    public long PaymentTransactionId { get; set; }
    public long OrderId { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? GatewayTransactionRef { get; set; }
    public decimal Amount { get; set; }
    public string TransactionStatus { get; set; } = null!;
    public DateTime? PaidAtUtc { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
