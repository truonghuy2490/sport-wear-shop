using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.AuthModels;

public class RegisterRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }
}