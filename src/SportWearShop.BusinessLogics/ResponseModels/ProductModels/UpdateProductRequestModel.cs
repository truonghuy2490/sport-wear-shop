using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels
{
    public class UpdateProductRequestModel
    {
        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Gender { get; set; }

        public string? BaseMaterial { get; set; }

        public string Status { get; set; } = "ACTIVE";
    }
}
