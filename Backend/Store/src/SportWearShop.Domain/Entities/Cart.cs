namespace SportWearShop.Domain.Entities;

public sealed class Cart : IUpdatedAtUtc
{
    public long CartId { get; set; }
    public long CustomerId { get; set; }
    public string CartStatus { get; set; } = null!;
    public long? ActiveCustomerId { get; private set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
