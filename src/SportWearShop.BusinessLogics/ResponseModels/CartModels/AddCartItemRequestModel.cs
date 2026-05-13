namespace SportWearShop.BusinessLogics.ResponseModels.CartModels;

public class AddCartItemRequestModel
{
    public long ProductVariantId { get; set; }

    public int Quantity { get; set; } = 1;
}