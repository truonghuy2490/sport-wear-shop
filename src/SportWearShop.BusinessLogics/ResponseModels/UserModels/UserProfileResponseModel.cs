namespace SportWearShop.BusinessLogics.ResponseModels.UserModels;

public class UserProfileResponseModel
{
    public long UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}