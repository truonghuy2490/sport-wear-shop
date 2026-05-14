
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels;
public class AdminProductDetailResponseModel
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string? BaseMaterial { get; set; }

    public long BrandId { get; set; }

    public string BrandName { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<AdminProductImageResponseModel> Images { get; set; } = [];

    public List<AdminProductVariantResponseModel> Variants { get; set; } = [];
}