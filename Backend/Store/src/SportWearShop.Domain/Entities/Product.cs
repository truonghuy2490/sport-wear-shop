namespace SportWearShop.Domain.Entities;

public sealed class Product : IUpdatedAtUtc
{
    public long ProductId { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? Gender { get; set; }
    public string? BaseMaterial { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
