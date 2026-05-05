namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class UpdateProductImageRequestModel
{
    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}