namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;

public class CheckoutItemResponseModel
{
    public string ProductName { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotalAmount { get; set; }
}