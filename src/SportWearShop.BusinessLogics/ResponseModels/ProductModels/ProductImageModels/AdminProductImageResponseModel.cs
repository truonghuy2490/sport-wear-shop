
namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class AdminProductImageResponseModel
{
    public long ProductImageId { get; set; }

    public long? ProductVariantId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}