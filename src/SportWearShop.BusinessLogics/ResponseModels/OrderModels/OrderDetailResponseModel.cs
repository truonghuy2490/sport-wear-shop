namespace SportWearShop.BusinessLogics.ResponseModels.OrderModels;
public class OrderDetailResponseModel
{
    public long OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string ShippingAddressSnapshot { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime OrderedAtUtc { get; set; }

    public List<OrderItemResponseModel> Items { get; set; } = [];
}