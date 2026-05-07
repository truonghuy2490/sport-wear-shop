using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.AuthModels;

public class AuthResponseModel
{
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
}