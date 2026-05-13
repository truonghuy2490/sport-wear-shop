namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;
public class CreateOrderFromCartRequestModel
{
    public long ShippingAddressId { get; set; }

    public string PaymentMethod { get; set; } = "COD"; // COD or Online(payos,vnpay,...)
}