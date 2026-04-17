using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class PaymentTransaction
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

    public virtual OrderHeader Order { get; set; } = null!;
}
