using Microsoft.AspNetCore.Http;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class UpdateProductImageRequestModel
{
    public IFormFile ImageFile { get; set; } = null!;

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
}