using System;
using System.Collections.Generic;
using System.Text;
namespace SportWearShop.Shared.ViewModels.AuthModels;


public class LoginRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}