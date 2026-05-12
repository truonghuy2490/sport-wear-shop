using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
public class AdminProductVariantResponseModel
{
    public long ProductVariantId { get; set; }

    public long ProductId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string? ColorCode { get; set; }

    public string? ColorName { get; set; }

    public string? SizeCode { get; set; }

    public string? SizeLabel { get; set; }

    public decimal ListPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public int? WeightGrams { get; set; }

    public string Status { get; set; } = string.Empty;

    public int QuantityOnHand { get; set; }

    public int QuantityReserved { get; set; }

    public int AvailableQuantity { get; set; }

    public List<AdminProductImageResponseModel> Images { get; set; } = [];
}