namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;
public class CreateOrderResponseModel
{
    public long OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentUrl { get; set; }
}