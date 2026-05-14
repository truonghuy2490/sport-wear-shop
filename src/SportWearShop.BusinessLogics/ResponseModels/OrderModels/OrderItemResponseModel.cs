namespace SportWearShop.BusinessLogics.ResponseModels.OrderModels;
public class OrderItemResponseModel
{
    public string SkuSnapshot { get; set; } = null!;

    public string ProductNameSnapshot { get; set; } = null!;

    public string ColorSnapshot { get; set; } = null!;

    public string SizeSnapshot { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotalAmount { get; set; }
}