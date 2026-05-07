namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class UpdateProductImageSortOrderRequestModel
{
    public long ProductImageId { get; set; }

    public int SortOrder { get; set; }
}