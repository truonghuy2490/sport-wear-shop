namespace SportWearShop.BusinessLogics.ResponseModels.UserModels;

public class UserDetailResponseModel
{
    public long UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool IsActive { get; set; }

    public List<string> Roles { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public List<UserAddressResponseModel> Addresses { get; set; } = new();
}