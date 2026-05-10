using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.AuthModels;


public class LoginRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}