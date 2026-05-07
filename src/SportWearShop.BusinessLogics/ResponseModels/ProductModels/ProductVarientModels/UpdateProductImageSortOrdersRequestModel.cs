using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;

public class UpdateProductImageSortOrdersRequestModel
{
    public List<UpdateProductImageSortOrderRequestModel> Images { get; set; } = [];
}