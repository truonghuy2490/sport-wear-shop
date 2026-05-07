
namespace SportWearShop.Shared.ViewModels.InventoryModels;

public class StockOutRequestModel
{
    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public long StaffId { get; set; } // claim id by JWT token later

    public string? Note { get; set; }
}