namespace SportWearShop.Repositories.ThirdPartyServices.ResultModels;
public class CloudinaryUploadResultModel
{
    public bool IsSuccess { get; set; }

    public string? ImageUrl { get; set; }
    public string? PublicId { get; set; }

    public string? ErrorMessage { get; set; }
}