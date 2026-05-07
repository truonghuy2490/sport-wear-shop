namespace SportWearShop.Shared.ViewModels.AuthModels;

public class RefreshTokenRequestModel
{
    public long UserId {get;set;}
    public string RefreshToken { get; set; } = null!;
}