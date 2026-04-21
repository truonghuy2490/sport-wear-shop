using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.Entities;

public class AppUser : IdentityUser<long>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }   

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}