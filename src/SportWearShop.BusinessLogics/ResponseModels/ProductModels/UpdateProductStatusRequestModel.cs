using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels;
public class UpdateProductStatusRequestModel
{
    public ProductStatus Status { get; set; }
}