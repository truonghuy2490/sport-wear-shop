namespace SportWearShop.BusinessLogics.ResponseModels.CartModels;

public class CartSummaryResponseModel
{
    public int TotalItems { get; set; }

    public int TotalQuantity { get; set; }

    public decimal SubTotal { get; set; }
}