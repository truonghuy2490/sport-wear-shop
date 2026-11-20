namespace SportWearShop.Domain.Entities;

public sealed class ProductImage
{
    public long ProductImageId { get; set; }
    public long ProductId { get; set; }
    public long? ProductVariantId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
