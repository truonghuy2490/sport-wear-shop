using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
public class UpdateProductVariantStatusRequestModel
{
    public ProductVariantStatus Status { get; set; }
}