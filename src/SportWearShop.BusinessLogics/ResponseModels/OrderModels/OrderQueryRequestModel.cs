namespace SportWearShop.BusinessLogics.ResponseModels.OrderModels;
public class OrderQueryRequestModel
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}