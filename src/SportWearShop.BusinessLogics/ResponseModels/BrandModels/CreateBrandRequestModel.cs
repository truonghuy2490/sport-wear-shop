using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SportWearShop.BusinessLogics.ResponseModels.BrandModels;

public class CreateBrandRequestModel
{
    public string BrandName { get; set; } = null!;

    public string BrandCode { get; set; } = null!;

    public IFormFile? BrandImageFile { get; set; }
}