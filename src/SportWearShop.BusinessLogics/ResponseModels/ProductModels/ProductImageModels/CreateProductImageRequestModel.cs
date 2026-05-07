<<<<<<< HEAD
using Microsoft.AspNetCore.Http;

=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class CreateProductImageRequestModel
{
    public long ProductId { get; set; }

    public long? ProductVariantId { get; set; }
<<<<<<< HEAD
    
    public IFormFile ImageFile { get; set; } = null!;

    public string? AltText { get; set; }

=======

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
    public bool IsPrimary { get; set; }
}