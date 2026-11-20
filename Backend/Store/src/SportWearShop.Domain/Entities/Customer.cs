namespace SportWearShop.Domain.Entities;

public sealed class Customer
{
    public long CustomerId { get; set; }
    public string IdentityIssuer { get; set; } = null!;
    public string IdentitySubject { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}
