<<<<<<< HEAD
using Microsoft.AspNetCore.Http;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class UpdateProductImageRequestModel
{
    public IFormFile ImageFile { get; set; } = null!;

    public string? AltText { get; set; }

=======
namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;
public class UpdateProductImageRequestModel
{
    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
    public bool IsPrimary { get; set; }
}