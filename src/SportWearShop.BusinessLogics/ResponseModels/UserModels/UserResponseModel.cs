using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.UserModels
{
    public class UserResponseModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> Roles { get; set; }
    }
}
