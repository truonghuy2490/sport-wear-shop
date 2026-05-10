
namespace SportWearShop.Shared.ViewModels.UserModels;

public class UserAddressResponseModel
{
    public long UserAddressId { get; set; }

    public string RecipientName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string City { get; set; } = null!;

    public string? Province { get; set; }

    public string? PostalCode { get; set; }

    public string CountryCode { get; set; } = null!;

    public bool IsDefaultShipping { get; set; }

    public bool IsDefaultBilling { get; set; }
}