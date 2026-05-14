using Microsoft.AspNetCore.Http;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class CreateProductImageRequestModel
{
    public long ProductId { get; set; }

    public long? ProductVariantId { get; set; }
    
    public IFormFile ImageFile { get; set; } = null!;

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
}