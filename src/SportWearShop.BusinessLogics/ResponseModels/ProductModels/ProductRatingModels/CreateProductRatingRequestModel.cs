using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels
{
    public class CreateProductRatingRequestModel
    {
        public long ProductId { get; set; }

        public long UserId { get; set; }

        public int RatingValue { get; set; }

        public string? ReviewText { get; set; }
    }
}
