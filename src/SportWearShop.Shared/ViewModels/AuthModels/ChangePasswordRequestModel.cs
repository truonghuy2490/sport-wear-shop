using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.AuthModels
{
    public class ChangePasswordRequestModel
    {
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
