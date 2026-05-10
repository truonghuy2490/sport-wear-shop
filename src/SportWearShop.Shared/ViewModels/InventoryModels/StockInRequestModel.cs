
namespace SportWearShop.Shared.ViewModels.InventoryModels;


public class StockInRequestModel
{
    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public long StaffId { get; set; } // claim id from JWT token later

    public string? Note { get; set; }
}