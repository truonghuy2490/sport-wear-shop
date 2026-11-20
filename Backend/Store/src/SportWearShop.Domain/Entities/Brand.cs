namespace SportWearShop.Domain.Entities;

public sealed class Brand : IUpdatedAtUtc
{
    public int BrandId { get; set; }
    public string BrandName { get; set; } = null!;
    public string BrandCode { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
