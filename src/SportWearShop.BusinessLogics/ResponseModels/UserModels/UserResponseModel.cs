using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.UserModels
{
    public class UserResponseModel
    {
        public long UserId { get; set; }

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }
    }
}
