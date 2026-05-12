using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.OrderModels;
public class OrderQueryRequestModel
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public OrderSortBy SortBy { get; set; } = OrderSortBy.OrderedAtUtc;

    public bool IsAscending { get; set; } = false;
}

//     OrderedAtUtc = 0,
//     OrderNumber = 1,
//     TotalAmount = 2,
//     OrderStatus = 3,
//     PaymentStatus = 4