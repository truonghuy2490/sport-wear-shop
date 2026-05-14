namespace SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;
public class ConfirmPaymentRequestModel
{
    public long OrderId { get; set; }

    public bool IsSuccess { get; set; }
}