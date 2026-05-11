namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;
public class CheckoutPreviewResponseModel
{
    public decimal SubtotalAmount { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal TotalAmount { get; set; }

    public List<CheckoutItemResponseModel> Items { get; set; } = [];
}