namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class CreateProductImageRequestModel
{
    public long ProductId { get; set; }

    public long? ProductVariantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}