namespace SportWearShop.BusinessLogics.ResponseModels.InventoryModels;


public class ReleaseStockRequestModel
{
    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public long OrderId { get; set; }

    public string? Note { get; set; }
}
