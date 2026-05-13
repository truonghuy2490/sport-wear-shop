using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels
{
    public class CreateProductRequestModel
    {
        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public ProductGender Gender { get; set; }

        public string? BaseMaterial { get; set; }

        public string? Slug { get; set; } // optional override
    }
}
