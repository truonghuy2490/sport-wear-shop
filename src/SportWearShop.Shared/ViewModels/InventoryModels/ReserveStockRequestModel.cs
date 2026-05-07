
namespace SportWearShop.Shared.ViewModels.InventoryModels;

public class ReserveStockRequestModel
{
    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public long OrderId { get; set; }

    public string? Note { get; set; }
}
