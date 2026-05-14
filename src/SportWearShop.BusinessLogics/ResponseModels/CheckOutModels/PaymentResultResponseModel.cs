namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;

public class PaymentResultResponseModel
{
    public long OrderId { get; set; }

    public bool IsSuccess { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;
}