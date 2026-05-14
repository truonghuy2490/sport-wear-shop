using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SportWearShop.BusinessLogics.ResponseModels.BrandModels;

public class UpdateBrandRequestModel
{
    public string? BrandName { get; set; }

    public string? BrandCode { get; set; }

    public IFormFile? BrandImageFile { get; set; }

    public bool? IsActive { get; set; }
}