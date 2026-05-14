namespace SportWearShop.BusinessLogics.ResponseModels.CartModels;

public class CartResponseModel
{
    public long CartId { get; set; }

    public long UserId { get; set; }

    public string CartStatus { get; set; } = null!;

    public List<CartItemResponseModel> Items { get; set; } = [];

    public int TotalItems { get; set; }

    public int TotalQuantity { get; set; }

    public decimal SubTotal { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}