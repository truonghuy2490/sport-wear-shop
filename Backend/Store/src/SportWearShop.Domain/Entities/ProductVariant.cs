namespace SportWearShop.Domain.Entities;

public sealed class ProductVariant : IUpdatedAtUtc
{
    public long ProductVariantId { get; set; }
    public long ProductId { get; set; }
    public string Sku { get; set; } = null!;
    public string ColorCode { get; set; } = null!;
    public string ColorName { get; set; } = null!;
    public string SizeCode { get; set; } = null!;
    public string SizeLabel { get; set; } = null!;
    public decimal ListPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int? WeightGrams { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
