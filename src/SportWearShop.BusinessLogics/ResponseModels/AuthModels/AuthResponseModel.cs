using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.AuthModels
{
    public class AuthResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public DateTime Expiration { get; set; }

        public string Email { get; set; }
        public string UserId { get; set; }
    }
}
