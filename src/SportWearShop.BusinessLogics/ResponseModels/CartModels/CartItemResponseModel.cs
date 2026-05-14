namespace SportWearShop.BusinessLogics.ResponseModels.CartModels;
public class CartItemResponseModel
{
    public long CartItemId { get; set; }

    public long ProductVariantId { get; set; }

    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public string ColorCode { get; set; } = null!;

    public string ColorName { get; set; } = null!;

    public string SizeCode { get; set; } = null!;

    public string SizeLabel { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public bool IsAvailable { get; set; }

    public int QuantityOnHand { get; set; }
}